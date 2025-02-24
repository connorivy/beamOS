//declare var Chart: typeof import("chart.js");

import { Chart, Colors, Filler, Plugin, LineController, LineElement, PointElement, LinearScale, CategoryScale, BarController, BarElement, Title, Tooltip, Legend, InteractionModeMap, ChartTypeRegistry, ChartConfiguration, ChartItem, ChartEvent, ActiveElement, ActiveDataPoint, Point, TooltipModel } from 'chart.js';

Chart.register(Colors, Filler, LineController, LineElement, PointElement, LinearScale, CategoryScale, BarController, BarElement, Title, Tooltip, Legend);

//function getGradient(ctx: CanvasRenderingContext2D, chartArea: { top: number, bottom: number, left: number, right: number }) {
//  const chartWidth = chartArea.right - chartArea.left;
//  const chartHeight = chartArea.bottom - chartArea.top;
//  let gradient = ctx.createLinearGradient(0, chartArea.bottom, 0, chartArea.top);
//  gradient.addColorStop(0, 'rgba(255, 99, 132, 0.5)');
//  gradient.addColorStop(0.5, 'rgba(54, 162, 235, 0.5)');
//  gradient.addColorStop(1, 'rgba(75, 192, 192, 0.5)');
//  return gradient;
//}

function createChart(canvasId: string) {
  const numData: number[] = Array.from({ length: 100 }, () => 50 - Math.floor(Math.random() * 100));
  const minValue = Math.min(...numData);
  const maxValue = Math.max(...numData);

  const chart1Canvas = document.getElementById('chart1') as HTMLCanvasElement;
  const ctx = chart1Canvas.getContext("2d");
  const gradient = ctx.createLinearGradient(0, 0, 0, 400);
  gradient.addColorStop(0, 'rgba(75, 192, 192, 1)'); // Start color
  gradient.addColorStop(1, 'rgba(75, 192, 192, 0)'); // End color

  ctx.fillStyle = gradient;
  ctx.fillRect(0, 0, chart1Canvas.width, chart1Canvas.height);

  // Sample data for the charts
  const data = {
    labels: Array.from({ length: 100 }, (_, i) => `Point ${i + 1}`), // 100 points
    datasets: [{
      label: 'Dataset 1',
      data: numData, // Random data for 100 points
      fill: true,
      //backgroundColor: (context: any) => {
      //  const chart = context.chart;
      //  const { ctx, chartArea } = chart;
      //  if (!chartArea) {
      //    // This case happens on initial chart load
      //    return null;
      //  }
      //  return getGradient(ctx, chartArea);
      //},
      borderColor: 'rgba(255, 99, 132, 1)',
      borderWidth: 2,
      pointRadius: numData.map((value) => (value === minValue || value === maxValue ? 5 : 0)), // Show circles every 10 points
      pointHoverRadius: numData.map((value) => (value === minValue || value === maxValue ? 10 : 0)) // Increase size on hover
    }]
  };

  // Shared state for the vertical line position
  let verticalLineX: number | null = null;
  let snapIndex: number | null = null;
  let hoveredIndex: number | null = null;

  // Custom plugin to draw the vertical line
  const verticalLinePlugin: Plugin = {
    id: 'verticalLinePlugin',
    afterLayout: (chart: Chart) => {
      /*** Set Gradient For Graph ***/

      var color = chart.data.datasets[0].borderColor as string;

      var zeroColor = Math.abs(chart.scales.y.min) / (Math.abs(chart.scales.y.max - chart.scales.y.min))
      var gradient = chart.ctx.createLinearGradient(0, chart.chartArea.bottom, 0, chart.chartArea.top);

      gradient.addColorStop(0, color.replace(/[\d\.]+\)$/g, '.5)'));
      gradient.addColorStop(zeroColor, color.replace(/[\d\.]+\)$/g, '0)'));
      gradient.addColorStop(1, color.replace(/[\d\.]+\)$/g, '.5)'));

      chart.data.datasets[0].backgroundColor = gradient;
    },
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
        intersect: false,
        mode: 'point',
      },
      hover: {
        mode: 'nearest',
        intersect: true // Set to true if you want the nearest point only when intersected
      },
      onHover: (event: ChartEvent, activeElements: ActiveElement[], chart: Chart) => {
        if (activeElements.length > 0) {
          snapIndex = activeElements[0].index;
        }
        else {
          snapIndex = null;
        }
      },
      plugins: {
        tooltip: {
          enabled: false,
          mode: 'index',
          intersect: false,
          external: (context) => {
            const tooltipEl = document.getElementById('custom-tooltip'+context.chart.id) as HTMLDivElement;

            // Hide if no tooltip
            if (hoveredIndex === null) {
              tooltipEl.style.display = 'none';
              return;
            }

            const label = data.labels[hoveredIndex];
            const value = data.datasets[0].data[hoveredIndex];

            // Set tooltip content
            tooltipEl.innerHTML = `
            <div><strong>${label}</strong></div>
            <div>Value: ${value}</div>
          `;

            //const xAxis = context.chart.scales.x;
            const x = context.chart.scales.x.getPixelForValue(hoveredIndex);
            const y = context.chart.scales.y.getPixelForValue(value);

            // Position the tooltip
            const { offsetLeft: positionX, offsetTop: positionY } = context.chart.canvas;

            tooltipEl.style.left = `${positionX + x + 5}px`;
            tooltipEl.style.top = `${positionY + y - 25}px`;
            tooltipEl.style.display = 'block';
          }
        },
        legend: {
          display: false
        }
      }
    },
    plugins: [verticalLinePlugin] // Add the custom plugin
  };

  // Initialize charts
  const chart1 = new Chart(chart1Canvas, config);
  const chart2 = new Chart(document.getElementById('chart2') as ChartItem, config);
  const chart3 = new Chart(document.getElementById('chart3') as ChartItem, config);

  const charts: Chart[] = [chart1, chart2, chart3];

  // Sync hover events
  const container = document.querySelector('.chart-container') as HTMLDivElement;

  container.addEventListener('mousemove', (event: MouseEvent) => {
    const xAxis = chart1.scales.x;

    if (snapIndex !== null) {
      hoveredIndex = snapIndex
    }
    else {
      let nearestIndex = 0;
      const rect = container.getBoundingClientRect();
      const mouseX = event.clientX - rect.left;

      // Get the x-axis scale and data points
      const dataPoints = chart1.data.labels!.length; // Use the number of labels as data points

      // Find the nearest data point
      let minDistance = Infinity;

      for (let i = 0; i < dataPoints; i++) {
        const pointX = xAxis.getPixelForValue(i); // Get the X pixel for the data point
        const distance = Math.abs(pointX - mouseX);

        if (distance < minDistance) {
          minDistance = distance;
          nearestIndex = i;
        }
      }

      hoveredIndex = nearestIndex;
    }

    const snapX = xAxis.getPixelForValue(hoveredIndex);

    // Update the vertical line position
    verticalLineX = snapX;

    // Redraw all charts to show the vertical line
    charts.forEach((chart: Chart) => {
      const tooltip = chart.tooltip;

      const activeElements: ActiveDataPoint[] = [{ datasetIndex: 0, index: hoveredIndex }];
      const point: Point = { x: event.clientX, y: event.clientY };

      // Set the active element
      tooltip.setActiveElements(activeElements, point);

      chart.update();
    });
  });

  container.addEventListener('mouseleave', () => {
    // Hide the vertical line when the mouse leaves the container
    verticalLineX = null;
    hoveredIndex = null;
    charts.forEach((chart: Chart) => {
      const tooltipEl = document.getElementById('custom-tooltip' + chart.id) as HTMLDivElement;
      tooltipEl.style.display = 'none';
      chart.update();
    });
  });
}

(<any>window).createCharts = createChart;
