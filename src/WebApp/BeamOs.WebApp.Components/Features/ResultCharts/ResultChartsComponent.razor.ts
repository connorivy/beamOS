//declare var Chart: typeof import("chart.js");

import { Chart, Colors, Plugin, LineController, LineElement, PointElement, LinearScale, CategoryScale, BarController, BarElement, Title, Tooltip, Legend, InteractionModeMap, ChartTypeRegistry, ChartConfiguration, ChartItem } from 'chart.js';

Chart.register(Colors, LineController, LineElement, PointElement, LinearScale, CategoryScale, BarController, BarElement, Title, Tooltip, Legend);

//export function createChart(canvasId: string) {
//  const plugin = {
//    id: 'corsair',
//    defaults: {
//      width: 1,
//      color: '#FF4949',
//      dash: [3, 3],
//    },
//    afterInit: (chart, args, opts) => {
//      chart.corsair = {
//        x: 0,
//        y: 0,
//      }
//    },
//    afterEvent: (chart, args) => {
//      const { inChartArea } = args
//      const { type, x, y } = args.event

//      chart.corsair = { x, y, draw: inChartArea }
//      chart.draw()
//    },
//    beforeDatasetsDraw: (chart, args, opts) => {
//      const { ctx } = chart
//      const { top, bottom, left, right } = chart.chartArea
//      if (!chart.corsair) {
//        chart.corsair = { x: 0, y: 0, draw: false };
//      }

//      const { x, y, draw } = chart.corsair
//      if (!draw) return

//      ctx.save()

//      ctx.beginPath()
//      ctx.lineWidth = opts.width
//      ctx.strokeStyle = opts.color
//      ctx.setLineDash(opts.dash)
//      ctx.moveTo(x, bottom)
//      ctx.lineTo(x, top)
//      ctx.moveTo(left, y)
//      ctx.lineTo(right, y)
//      ctx.stroke()

//      ctx.restore()
//    }
//  }

//  const data = {
//    labels: ["Red", "Blue", "Yellow", "Green", "Purple", "Orange"],
//    datasets: [{
//      label: '# of Votes',
//      data: [12, 19, 3, 5, 2, 3],
//      borderWidth: 1,
//      pointHitRadius: 10
//    },
//    {
//      label: '# of Points',
//      data: [7, 11, 5, 8, 3, 7],
//      borderWidth: 1,
//      pointHitRadius: 10
//    }
//    ]
//  }
//  const options = {
//    maintainAspectRatio: false,
//    hover: {
//      mode: 'nearest',
//      intersect: true,
//    },
//    plugins: {
//      corsair: {
//        color: 'black',
//      }
//    }
//  }

//  const config = {
//    type: 'line',
//    data,
//    options,
//    plugins: [plugin],
//  }

//  const $chart = document.getElementById(canvasId)
//  const chart = new Chart($chart, config)
//}

//window.createChart = createChart;

function createChart(canvasId: string) {
  // Sample data for the charts
  const data = {
    labels: Array.from({ length: 100 }, (_, i) => `Point ${i + 1}`), // 500 points
    datasets: [{
      label: 'Dataset 1',
      data: Array.from({ length: 100 }, () => Math.floor(Math.random() * 100)), // Random data for 500 points
      backgroundColor: 'rgba(255, 99, 132, 0.2)',
      borderColor: 'rgba(255, 99, 132, 1)',
      borderWidth: 1,
      pointRadius: Array.from({ length: 100 }, (_, i) => (i % 10 === 0 ? 3 : 0)), // Show circles every 10 points
      pointHoverRadius: Array.from({ length: 100 }, (_, i) => (i % 10 === 0 ? 5 : 3)) // Larger hover radius for visible points
    }]
  };

  // Shared state for the vertical line position
  let verticalLineX: number | null = null;
  let hoveredIndex: number | null = null; // Track the currently hovered point

  // Custom plugin to draw the vertical line
  const verticalLinePlugin: Plugin = {
    id: 'verticalLinePlugin',
    afterDraw: (chart: Chart) => {
      if (verticalLineX !== null) {
        const ctx = chart.ctx;
        const yAxis = chart.scales.y;

        ctx.save();
        ctx.beginPath();
        ctx.moveTo(verticalLineX, yAxis.top);
        ctx.lineTo(verticalLineX, yAxis.bottom);
        ctx.strokeStyle = 'red';
        ctx.lineWidth = 1;
        ctx.stroke();
        ctx.restore();
      }
    }
  };

  // Chart configuration
  const config: ChartConfiguration = {
    type: 'line',
    data: data,
    options: {
      responsive: true,
      interaction: {
        mode: 'index',
        intersect: false
      },
      plugins: {
        tooltip: {
          enabled: true,
          mode: 'index',
          intersect: false
        }
      }
    },
    plugins: [verticalLinePlugin] // Add the custom plugin
  };

  // Initialize charts
  const chart1 = new Chart(document.getElementById('chart1') as ChartItem, config);
  const chart2 = new Chart(document.getElementById('chart2') as ChartItem, config);
  const chart3 = new Chart(document.getElementById('chart3') as ChartItem, config);

  const charts: Chart[] = [chart1, chart2, chart3];

  // Sync hover events
  const container = document.querySelector('.chart-container') as HTMLDivElement;

  container.addEventListener('mousemove', (event: MouseEvent) => {
    const rect = container.getBoundingClientRect();
    const mouseX = event.clientX - rect.left;

    // Get the x-axis scale and data points
    const xAxis = chart1.scales.x;
    const dataPoints = chart1.data.labels!.length; // Use the number of labels as data points

    // Check if the mouse is still within the hover radius of the currently hovered point
    if (hoveredIndex !== null) {
      const dataset = chart1.data.datasets[0];
      const pointHoverRadius = (<any>dataset).pointHoverRadius;

      // Handle case where pointHoverRadius is a number or an array
      const hoverRadius = Array.isArray(pointHoverRadius)
        ? pointHoverRadius[hoveredIndex]
        : pointHoverRadius;

      const pointX = xAxis.getPixelForValue(hoveredIndex);
      const distance = Math.abs(pointX - mouseX);

      // If the mouse is within the hover radius, keep the vertical line snapped to this point
      if (distance <= hoverRadius) {
        verticalLineX = pointX;
        charts.forEach((chart: Chart) => chart.update());
        return;
      }
    }

    // Find the nearest data point
    let nearestIndex = 0;
    let minDistance = Infinity;

    for (let i = 0; i < dataPoints; i++) {
      const pointX = xAxis.getPixelForValue(i); // Get the X pixel for the data point
      const distance = Math.abs(pointX - mouseX);

      if (distance < minDistance) {
        minDistance = distance;
        nearestIndex = i;
      }
    }

    // Update the currently hovered point
    hoveredIndex = nearestIndex;

    // Update the vertical line position
    verticalLineX = xAxis.getPixelForValue(nearestIndex);

    // Redraw all charts to show the vertical line
    charts.forEach((chart: Chart) => {
      chart.update();
    });
  });

  container.addEventListener('mouseleave', () => {
    // Hide the vertical line when the mouse leaves the container
    verticalLineX = null;
    hoveredIndex = null;
    charts.forEach((chart: Chart) => {
      chart.update();
    });
  });
}

(<any>window).createCharts = createChart;
