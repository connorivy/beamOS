/* eslint-disable @typescript-eslint/no-non-null-assertion */
/* eslint-disable @typescript-eslint/restrict-plus-operands */
/* eslint-disable @typescript-eslint/restrict-template-expressions */
/* eslint-disable @typescript-eslint/no-unsafe-call */
/* eslint-disable @typescript-eslint/no-unsafe-assignment */
/* eslint-disable @typescript-eslint/no-unsafe-member-access */
/* eslint-disable @typescript-eslint/no-explicit-any */
//declare var Chart: typeof import("chart.js");

import type { Plugin, ChartConfiguration, ChartEvent, ActiveElement, ActiveDataPoint, Point } from 'chart.js';
import { Chart, Colors, Filler, LineController, LineElement, PointElement, LinearScale, CategoryScale, BarController, BarElement, Title, Tooltip, Legend, InteractionModeMap, ChartTypeRegistry, ChartItem, TooltipModel } from 'chart.js';
import { useEffect, useRef, useState } from 'react';
import { useAppSelector } from '../../../app/hooks';
import { selectModelResponseByCanvasId } from '../../editors/editorsSlice';
import { element1dIdSelector } from '../../editors/selection-info/element1d/element1dSelectionSlice';

Chart.register(Colors, Filler, LineController, LineElement, PointElement, LinearScale, CategoryScale, BarController, BarElement, Title, Tooltip, Legend);

//Chart.defaults.color = "#fff";

export const Element1dResultCharts: React.FC<{ canvasId: string }> = ({ canvasId }) => {
  // Chart state and refs
  const chart1Ref = useRef<Chart | null>(null);
  const chart2Ref = useRef<Chart | null>(null);
  const chart3Ref = useRef<Chart | null>(null);

  const [verticalLineX, setVerticalLineX] = useState<number | null>(null);
  const [snapIndex, setSnapIndex] = useState<number | null>(null);
  const [hoveredIndex, setHoveredIndex] = useState<number | null>(null);

  // Canvas and tooltip refs
  const chart1CanvasRef = useRef<HTMLCanvasElement | null>(null);
  const chart2CanvasRef = useRef<HTMLCanvasElement | null>(null);
  const chart3CanvasRef = useRef<HTMLCanvasElement | null>(null);
  const tooltip1Ref = useRef<HTMLDivElement | null>(null);
  const tooltip2Ref = useRef<HTMLDivElement | null>(null);
  const tooltip3Ref = useRef<HTMLDivElement | null>(null);

  // Container ref
  const containerRef = useRef<HTMLDivElement | null>(null);

  const modelState = useAppSelector(
    state => selectModelResponseByCanvasId(state, canvasId)
  )
  const element1dId = useAppSelector(element1dIdSelector)
  const currentResultSet = modelState?.resultSets[1]
  if (!modelState || !element1dId || !currentResultSet) {
    return <div>No data available for the selected Element1d.</div>
  }

  const shearDiagram = currentResultSet.shearDiagrams?.find(diagram => diagram.element1dId === element1dId)
  const momentDiagram = currentResultSet.momentDiagrams?.find(diagram => diagram.element1dId === element1dId)
  if (!shearDiagram) {
    return <div>No shear data available for the selected Element1d.</div>
  }
  if (!momentDiagram) {
    return <div>No moment data available for the selected Element1d.</div>
  }

  // Dummy data for demonstration (replace with props or context as needed)
  const chartXValues: number[] = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
  const shearValues: number[] = [0, 2, 4, 3, 2, 1, 0, -1, -2, -3, -4];
  const momentValues: number[] = [0, 1, 2, 1, 0, -1, -2, -1, 0, 1, 2];
  const deflectionValues: number[] = [0, 0.5, 1, 0.8, 0.6, 0.4, 0.2, 0, -0.2, -0.4, -0.6];

  // Helper for chart data
  function getData(axisLabels: number[], data: number[], color: string): { labels: number[]; datasets: any[] } {
    const minValue = Math.min(...data);
    const maxValue = Math.max(...data);
    const pointRad = 5;
    const pointHoverRad = 10;
    const pointRads: number[] = [];
    const pointHoverRads: number[] = [];
    let foundMax = false;
    let foundMin = false;
    // for (let i = 0; i < data.length; i++) {
    for (const currentData of data) {
      if (currentData === maxValue) {
        foundMin = false;
        if (!foundMax) {
          pointRads.push(pointRad);
          pointHoverRads.push(pointHoverRad);
          foundMax = true;
        } else {
          pointRads.push(0);
          pointHoverRads.push(0);
        }
      } else if (currentData === minValue) {
        foundMax = false;
        if (!foundMin) {
          pointRads.push(pointRad);
          pointHoverRads.push(pointHoverRad);
          foundMin = true;
        } else {
          pointRads.push(0);
          pointHoverRads.push(0);
        }
      } else {
        pointRads.push(0);
        pointHoverRads.push(0);
        foundMax = false;
        foundMin = false;
      }
    }
    return {
      labels: axisLabels,
      datasets: [{
        label: 'Shear Diagram',
        data: data,
        fill: true,
        borderColor: color,
        borderWidth: 2,
        pointRadius: pointRads,
        pointHoverRadius: pointHoverRads
      }]
    }
  }

  // Plugin for vertical line and gradient
  const verticalLinePlugin: Plugin = {
    id: 'verticalLinePlugin',
    afterLayout: (chart: Chart) => {
      const color = chart.data.datasets[0].borderColor as string;
      const zeroColor = Math.abs(chart.scales.y.min) / (Math.abs(chart.scales.y.max - chart.scales.y.min));
      const gradient = chart.ctx.createLinearGradient(0, chart.chartArea.bottom, 0, chart.chartArea.top);
      gradient.addColorStop(0, color.replace(/\d+\.?\d*\)$/g, '.5)'));
      if (0 <= zeroColor && zeroColor <= 1) {
        gradient.addColorStop(zeroColor, color.replace(/\d+\.?\d*\)$/g, '0)'));
      }
      gradient.addColorStop(1, color.replace(/\d+\.?\d*\)$/g, '.5)'));
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
        ctx.strokeStyle = 'black';
        ctx.lineWidth = 1;
        ctx.stroke();
        ctx.restore();
      }
    }
  };

  // Chart config helper
  function getConfig(axisLabels: number[], yData: number[], color: string, yAxisLabel: string): ChartConfiguration {
    const data = getData(axisLabels, yData, color);
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
          intersect: true
        },
        onHover: (_event: ChartEvent, activeElements: ActiveElement[]) => {
          if (activeElements.length > 0) {
            setSnapIndex(activeElements[0].index);
          } else {
            setSnapIndex(null);
          }
        },
        scales: {
          x: {
            type: 'linear',
            max: axisLabels.slice(-1)[0],
            ticks: {
              maxTicksLimit: 11,
              precision: 3,
            }
          },
          y: {
            type: 'linear',
            afterFit: function (scale: any) {
              scale.width = 65;
            },
            ticks: {
              precision: 3
            },
            title: {
              display: true,
              text: yAxisLabel,
            }
          }
        },
        plugins: {
          tooltip: {
            enabled: false,
            mode: 'index',
            intersect: false,
            external: (context: any) => {
              const chartIndex = context.chart.canvas.id.slice(-1);
              const tooltipRefs = [tooltip1Ref, tooltip2Ref, tooltip3Ref];
              // eslint-disable-next-line @typescript-eslint/no-unsafe-argument
              const tooltipEl = tooltipRefs[parseInt(chartIndex) - 1]?.current;
              if (!tooltipEl) return;
              if (hoveredIndex === null) {
                tooltipEl.style.display = 'none';
                return;
              }
              const label = data.labels[hoveredIndex];
              const value = data.datasets[0].data[hoveredIndex];
              tooltipEl.innerHTML = `
                <div><strong>${label.toFixed(4)}</strong></div>
                <div>Value: ${value.toFixed(4)}</div>
              `;
              const x = context.chart.scales.x.getPixelForValue(label);
              const y = context.chart.scales.y.getPixelForValue(value);
              const { offsetLeft: positionX, offsetTop: positionY } = context.chart.canvas;
              if (hoveredIndex < data.labels.length / 2) {
                tooltipEl.style.left = `${positionX + x + 5}px`;
                tooltipEl.classList.remove('flipped');
              } else {
                const tooltipWidth = tooltipEl.offsetWidth;
                tooltipEl.classList.add('flipped');
                tooltipEl.style.left = `${positionX + x - 5 - tooltipWidth}px`;
              }
              tooltipEl.style.top = `${positionY + y - 25}px`;
              tooltipEl.style.display = 'block';
            }
          },
          legend: {
            display: false
          }
        }
      },
      plugins: [verticalLinePlugin]
    };
  }

  // Chart creation and event listeners
  useEffect(() => {
    // Destroy charts if they exist
    chart1Ref.current?.destroy();
    chart2Ref.current?.destroy();
    chart3Ref.current?.destroy();
    chart1Ref.current = null;
    chart2Ref.current = null;
    chart3Ref.current = null;

    // Create charts
    if (chart1CanvasRef.current) {
      chart1Ref.current = new Chart(chart1CanvasRef.current, getConfig(chartXValues, shearValues, 'rgba(255,199,0,1)', 'Shear'));
    }
    if (chart2CanvasRef.current) {
      chart2Ref.current = new Chart(chart2CanvasRef.current, getConfig(chartXValues, momentValues, 'rgba(6,120,255,1)', 'Moment'));
    }
    if (chart3CanvasRef.current) {
      chart3Ref.current = new Chart(chart3CanvasRef.current, getConfig(chartXValues, deflectionValues, 'rgba(33, 166, 81,1)', 'Deflection'));
    }

    // Mousemove and mouseleave event listeners
    const container = containerRef.current;
    if (!container) return;

    const handleMouseMove = (event: MouseEvent) => {
      const chart1 = chart1Ref.current;
      if (!chart1) {
        setHoveredIndex(null);
        setSnapIndex(null);
        return;
      }
      const xAxis = chart1.scales.x;
      const dataPoints = chart1.data.labels!.length;
      let nearestIndex = 0;
      const rect = container.getBoundingClientRect();
      const mouseX = event.clientX - rect.left;
      let minDistance = Infinity;
      for (let i = 0; i < dataPoints; i++) {
        const xVal = Number(chart1.data.labels![i]);
        const pointX = xAxis.getPixelForValue(xVal);
        const distance = Math.abs(pointX - mouseX);
        if (distance < minDistance) {
          minDistance = distance;
          nearestIndex = i;
        }
      }
      if (snapIndex !== null) {
        setHoveredIndex(snapIndex);
      } else {
        setHoveredIndex(nearestIndex);
      }
      const xVal = Number(chart1.data.labels![snapIndex ?? nearestIndex]);
      const snapX = xAxis.getPixelForValue(xVal);
      setVerticalLineX(snapX);
      [chart1Ref.current, chart2Ref.current, chart3Ref.current].forEach((chart) => {
        if (!chart) return;
        const tooltip = chart.tooltip;
        const activeElements: ActiveDataPoint[] = [{ datasetIndex: 0, index: snapIndex ?? nearestIndex }];
        const point: Point = { x: event.clientX, y: event.clientY };
        tooltip?.setActiveElements(activeElements, point);
        chart.update();
      });
    };

    const handleMouseLeave = () => {
      setVerticalLineX(null);
      setHoveredIndex(null);
      [chart1Ref.current, chart2Ref.current, chart3Ref.current].forEach((chart, idx) => {
        if (!chart) return;
        const tooltipRefs = [tooltip1Ref, tooltip2Ref, tooltip3Ref];
        const tooltipEl = tooltipRefs[idx]?.current;
        if (tooltipEl) tooltipEl.style.display = 'none';
        chart.update();
      });
    };

    container.addEventListener('mousemove', handleMouseMove);
    container.addEventListener('mouseleave', handleMouseLeave);

    // Cleanup
    return () => {
      container.removeEventListener('mousemove', handleMouseMove);
      container.removeEventListener('mouseleave', handleMouseLeave);
      chart1Ref.current?.destroy();
      chart2Ref.current?.destroy();
      chart3Ref.current?.destroy();
    };
  }, [chartXValues, shearValues, momentValues, deflectionValues, snapIndex]);

  return (
    <div className="chart-container" ref={containerRef} style={{ position: 'relative', width: '100%', height: '400px' }}>
      <canvas id="chart1" ref={chart1CanvasRef} />
      <div id="custom-tooltip1" ref={tooltip1Ref} style={{ position: 'absolute', display: 'none' }} />
      <canvas id="chart2" ref={chart2CanvasRef} />
      <div id="custom-tooltip2" ref={tooltip2Ref} style={{ position: 'absolute', display: 'none' }} />
      <canvas id="chart3" ref={chart3CanvasRef} />
      <div id="custom-tooltip3" ref={tooltip3Ref} style={{ position: 'absolute', display: 'none' }} />
    </div>
  );
}