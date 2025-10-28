// import type { ActiveElement, ChartConfiguration, ChartEvent, Scale } from "chart.js";
import type { ActiveElement, ChartConfiguration, ChartEvent, Scale } from "chart.js";
import { BarController, BarElement, CategoryScale, Chart, Colors, Filler, Legend, LinearScale, LineController, LineElement, PointElement, Title, Tooltip } from "chart.js";
import type { Plugin as ChartPlugin } from "chart.js";
import { useEffect, useRef } from "react";

Chart.register(Colors, Filler, LineController, LineElement, PointElement, LinearScale, CategoryScale, BarController, BarElement, Title, Tooltip, Legend);


const verticalLinePlugin: ChartPlugin = {
    id: 'verticalLinePlugin',
    afterLayout: (chart: Chart) => {
        /*** Set Gradient For Graph ***/
        const color = chart.data.datasets[0].borderColor as string;

        const zeroColor = Math.abs(chart.scales.y.min) / (Math.abs(chart.scales.y.max - chart.scales.y.min))
        console.log("Zero Color:", zeroColor);
        const gradient = chart.ctx.createLinearGradient(0, chart.chartArea.bottom, 0, chart.chartArea.top);

        gradient.addColorStop(0, color.replace(/[\d.]+\)$/g, '.5)'));

        if (0 <= zeroColor && zeroColor <= 1) {
            gradient.addColorStop(zeroColor, color.replace(/[\d.]+\)$/g, '0)'));
        }
        gradient.addColorStop(1, color.replace(/[\d.]+\)$/g, '.5)'));

        chart.data.datasets[0].backgroundColor = gradient;
    },
    afterDraw: (chart: Chart) => {
        // if (this.verticalLineX !== null) {
        //     const ctx = chart.ctx;
        //     const yAxis = chart.scales.y;

        //     ctx.save();
        //     ctx.beginPath();
        //     ctx.moveTo(this.verticalLineX, yAxis.top);
        //     ctx.lineTo(this.verticalLineX, yAxis.bottom);
        //     ctx.strokeStyle = 'black';
        //     ctx.lineWidth = 1;
        //     ctx.stroke();
        //     ctx.restore();
        // }
    }
};

function getData(yAxisLabel: string, axisLabels: number[], data: number[], color: string): {
    labels: number[]; datasets: {
        label: string,
        data: number[],
        backgroundColor?: string,
        borderColor?: string,
        borderWidth?: number,
        pointRadius?: number | number[],
        pointHoverRadius?: number | number[],
        fill?: boolean
    }[]
} {
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
            label: yAxisLabel,
            data: data,
            fill: true,
            borderColor: color,
            backgroundColor: color,
            borderWidth: 2,
            pointRadius: pointRads,
            pointHoverRadius: pointHoverRads
        }]
    }
}
function getConfig(axisLabels: number[], yData: number[], color: string, yAxisLabel: string): ChartConfiguration {
    const data = getData(yAxisLabel, axisLabels, yData, color);
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
                // if (activeElements.length > 0) {
                //     setSnapIndex(activeElements[0].index);
                // } else {
                //     setSnapIndex(null);
                // }
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
                    afterFit: function (scale: Scale) {
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
            // plugins: {
            //     tooltip: {
            //         enabled: true,
            //         mode: 'index',
            //         intersect: false,
            //     },
            //     legend: {
            //         display: false
            //     }
            // }
            plugins: {
                tooltip: {
                    enabled: true,
                    mode: 'index',
                    intersect: false,
                    //     external: (context: any) => {
                    //         const chartIndex = context.chart.canvas.id.slice(-1);
                    //         const tooltipRefs = [tooltip1Ref, tooltip2Ref, tooltip3Ref];
                    //         // eslint-disable-next-line @typescript-eslint/no-unsafe-argument
                    //         const tooltipEl = tooltipRefs[parseInt(chartIndex) - 1]?.current;
                    //         if (!tooltipEl) return;
                    //         if (hoveredIndex === null) {
                    //             tooltipEl.style.display = 'none';
                    //             return;
                    //         }
                    //         const label = data.labels[hoveredIndex];
                    //         const value = data.datasets[0].data[hoveredIndex];
                    //         tooltipEl.innerHTML = `
                    //     <div><strong>${label.toFixed(4)}</strong></div>
                    //     <div>Value: ${value.toFixed(4)}</div>
                    //   `;
                    //         const x = context.chart.scales.x.getPixelForValue(label);
                    //         const y = context.chart.scales.y.getPixelForValue(value);
                    //         const { offsetLeft: positionX, offsetTop: positionY } = context.chart.canvas;
                    //         if (hoveredIndex < data.labels.length / 2) {
                    //             tooltipEl.style.left = `${positionX + x + 5}px`;
                    //             tooltipEl.classList.remove('flipped');
                    //         } else {
                    //             const tooltipWidth = tooltipEl.offsetWidth;
                    //             tooltipEl.classList.add('flipped');
                    //             tooltipEl.style.left = `${positionX + x - 5 - tooltipWidth}px`;
                    //         }
                    //         tooltipEl.style.top = `${positionY + y - 25}px`;
                    //         tooltipEl.style.display = 'block';
                    //     }
                },
                legend: {
                    display: false
                }
            }
        },
        plugins: [verticalLinePlugin]
    }
    // }
}

export const LineChartComponent: React.FC<{
    xValues: number[],
    yValues: number[],
    lineColor: string,
    yAxisLabel: string
}> = ({ xValues, yValues, lineColor, yAxisLabel }) => {
    const canvasRef = useRef<HTMLCanvasElement>(null);

    useEffect(() => {
        if (!canvasRef.current) {
            return;
        }
        const ctx = canvasRef.current
        const config = getConfig(xValues, yValues, lineColor, yAxisLabel)
        const chart = new Chart(ctx, config);

        return () => {
            chart.destroy();
        };

    }, [lineColor, xValues, yAxisLabel, yValues]);

    return (
        <canvas id="chart1" ref={canvasRef} />
    );
}

export const ExampleLineChartComponent: React.FC = () => {
    return (
        <LineChartComponent
            xValues={[0, 1, 2, 3, 4, 5, 6]}
            yValues={[0, 10, 5, 2, 20, 30, 45]}
            lineColor="rgba(255,199,0,1)"
            yAxisLabel="Shear Force (kN)"
        />
    );
}