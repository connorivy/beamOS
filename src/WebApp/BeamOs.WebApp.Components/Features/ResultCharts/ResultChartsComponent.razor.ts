//declare var Chart: typeof import("chart.js");

import { Chart, Colors, Filler, Plugin, LineController, LineElement, PointElement, LinearScale, CategoryScale, BarController, BarElement, Title, Tooltip, Legend, InteractionModeMap, ChartTypeRegistry, ChartConfiguration, ChartItem, ChartEvent, ActiveElement, ActiveDataPoint, Point, TooltipModel } from 'chart.js';

Chart.register(Colors, Filler, LineController, LineElement, PointElement, LinearScale, CategoryScale, BarController, BarElement, Title, Tooltip, Legend);

class ResultCharts {

  private verticalLineX: number | null = null;
  private snapIndex: number | null = null;
  private hoveredIndex: number | null = null;

  private chart1: Chart | null = null;
  private chart2: Chart | null = null;
  private chart3: Chart | null = null;

  private charts: Chart[] = [this.chart1, this.chart2, this.chart3];

  private isInit: boolean;

  private Init() {
    if (this.isInit) {
      return;
    }
    this.isInit = true;

    const container = document.querySelector('.chart-container') as HTMLDivElement;

    container.addEventListener('mousemove', (event: MouseEvent) => {
      if (this.chart1 === null) {
        this.hoveredIndex = null;
        this.snapIndex = null;
        return;
      }

      const xAxis = this.chart1.scales.x;

      if (this.snapIndex !== null) {
        this.hoveredIndex = this.snapIndex
      }
      else {
        let nearestIndex = 0;
        const rect = container.getBoundingClientRect();
        const mouseX = event.clientX - rect.left;

        // Get the x-axis scale and data points
        const dataPoints = this.chart1.data.labels!.length; // Use the number of labels as data points

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

        this.hoveredIndex = nearestIndex;
      }

      const snapX = xAxis.getPixelForValue(this.hoveredIndex);

      // Update the vertical line position
      this.verticalLineX = snapX;

      // Redraw all charts to show the vertical line
      this.charts.forEach((chart: Chart) => {
        const tooltip = chart.tooltip;

        const activeElements: ActiveDataPoint[] = [{ datasetIndex: 0, index: this.hoveredIndex }];
        const point: Point = { x: event.clientX, y: event.clientY };

        // Set the active element
        tooltip?.setActiveElements(activeElements, point);

        chart.update();
      });
    });

    container.addEventListener('mouseleave', () => {
      // Hide the vertical line when the mouse leaves the container
      this.verticalLineX = null;
      this.hoveredIndex = null;

      if (this.chart1 === null) {
        return;
      }

      this.charts.forEach((chart: Chart) => {
        const chartIndex = chart.canvas.id.slice(-1);
        const tooltipEl = document.getElementById('custom-tooltip' + chartIndex) as HTMLDivElement;
        tooltipEl.style.display = 'none';
        chart.update();
      });
    });
  }


  static getData(axisLabels: number[], data: number[], color: string) {
    const minValue = Math.min(...data);
    const maxValue = Math.max(...data);

    return {
      labels: axisLabels,
      datasets: [{
        label: 'Shear Diagram',
        data: data,
        //data: shearData,
        fill: true,
        borderColor: color,
        borderWidth: 2,
        pointRadius: data.map((value) => (value === minValue || value === maxValue ? 5 : 0)),
        pointHoverRadius: data.map((value) => (value === minValue || value === maxValue ? 10 : 0))
      }]
    }
  }

  public createCharts(xValues: number[], shearData: number[], momentData: number[], deflectionData: number[]) {
    this.Init();
    this.destroyCharts();
  
    this.chart1 = new Chart(document.getElementById('chart1') as ChartItem, this.getConfig(xValues, shearData, 'rgba(255,199,0,1)'));
    this.chart2 = new Chart(document.getElementById('chart2') as ChartItem, this.getConfig(xValues, momentData, 'rgba(6,120,255,1)'));
    this.chart3 = new Chart(document.getElementById('chart3') as ChartItem, this.getConfig(xValues, deflectionData, 'rgba(33, 166, 81,1)'));

    this.charts = [this.chart1, this.chart2, this.chart3];
  }

  private verticalLinePlugin: Plugin = {
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
      if (this.verticalLineX !== null) {
        const ctx = chart.ctx;
        const yAxis = chart.scales.y;

        ctx.save();
        ctx.beginPath();
        ctx.moveTo(this.verticalLineX, yAxis.top);
        ctx.lineTo(this.verticalLineX, yAxis.bottom);
        ctx.strokeStyle = 'black';
        ctx.lineWidth = 1;
        ctx.stroke();
        ctx.restore();
      }
    }
  };

  private getConfig(axisLabels: number[], yData: number[], color: string): ChartConfiguration {
    const data = ResultCharts.getData(axisLabels, yData, color);

    return {
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
            this.snapIndex = activeElements[0].index;
          }
          else {
            this.snapIndex = null;
          }
        },
        plugins: {
          tooltip: {
            enabled: false,
            mode: 'index',
            intersect: false,
            external: (context) => {
              const chartIndex = context.chart.canvas.id.slice(-1);
              const tooltipEl = document.getElementById('custom-tooltip' + chartIndex) as HTMLDivElement;

              // Hide if no tooltip
              if (this.hoveredIndex === null) {
                tooltipEl.style.display = 'none';
                return;
              }

              const label = data.labels[this.hoveredIndex];
              const value = data.datasets[0].data[this.hoveredIndex];

              // Set tooltip content
              tooltipEl.innerHTML = `
                <div><strong>${label}</strong></div>
                <div>Value: ${value}</div>
              `;

              //const xAxis = context.chart.scales.x;
              const x = context.chart.scales.x.getPixelForValue(this.hoveredIndex);
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
      plugins: [this.verticalLinePlugin] // Add the custom plugin
    };
  }

  public destroyCharts() {
    this.chart1?.destroy();
    this.chart2?.destroy();
    this.chart3?.destroy();

    this.chart1 = null;
    this.chart2 = null;
    this.chart3 = null;
  }
}

(<any>window).resultCharts = new ResultCharts();
