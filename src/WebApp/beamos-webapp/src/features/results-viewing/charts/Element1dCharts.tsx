import type { ReactNode } from 'react';
import type React from 'react';
import { useEffect, useRef } from 'react';
import { useAppSelector } from '../../../app/hooks';
import { selectModelResponseByCanvasId } from '../../editors/editorsSlice';
import { LineChartComponent } from './Chart';
import { getValueAtLocation } from './diagramConsistantIntervalExtensions';
import { LengthUnit } from '../../../utils/type-extensions/UnitTypeContracts';


//Chart.defaults.color = "#fff";

export const Element1dResultCharts: React.FC<{ canvasId: string, element1dId: number, resultSetId: number }> = ({ canvasId, element1dId, resultSetId }) => {
  const modelState = useAppSelector(
    state => selectModelResponseByCanvasId(state, canvasId)
  )
  const currentResultSet = modelState?.resultSets[resultSetId]
  const shearDiagramRef = useRef<ReactNode | null>(null);
  const momentDiagramRef = useRef<ReactNode | null>(null);
  const deflectionDiagramRef = useRef<ReactNode | null>(null);

  // Chart creation and event listeners
  useEffect(() => {
    console.log("Rendering Element1dResultCharts useEffect");
    if (!modelState || !element1dId || !currentResultSet) {
      console.log("Missing modelState, element1dId, or currentResultSet");
      return
    }

    const shearDiagram = currentResultSet.shearDiagrams?.find(diagram => diagram.element1dId === element1dId)
    const momentDiagram = currentResultSet.momentDiagrams?.find(diagram => diagram.element1dId === element1dId)
    const deflectionDiagram = currentResultSet.deflectionDiagrams?.find(diagram => diagram.element1dId === element1dId)
    if (!shearDiagram) {
      console.log("No shear diagram found");
      return
    }
    if (!momentDiagram) {
      console.log("No moment diagram found");
      return
    }
    if (!deflectionDiagram) {
      console.log("No deflection diagram found");
      return
    }

    const element1dLength = shearDiagram.intervals[shearDiagram.intervals.length - 1].endLocation.value;
    // Generate regular interval locations
    const regularIntervalLocations = Array.from(
      { length: deflectionDiagram.numSteps },
      (_, i) => i * (element1dLength / (deflectionDiagram.numSteps - 1))
    ).sort((a, b) => a - b); // Sorted set

    // Build relativeOffsets array
    const relativeOffsets: number[] = [];
    for (let i = 0; i < regularIntervalLocations.length; i++) {
      relativeOffsets.push(deflectionDiagram.offsets[i * 3 + 1]);
    }

    // Build evalPoints array
    const shearPoints = shearDiagram.intervals.flatMap(i => [i.startLocation.value, i.endLocation.value]);
    const momentPoints = momentDiagram.intervals.flatMap(i => [i.startLocation.value, i.endLocation.value]);
    const evalPoints = Array.from(
      new Set([...shearPoints, ...momentPoints, ...regularIntervalLocations])
    ).sort((a, b) => a - b);

    const shearValues: number[] = [];
    const momentValues: number[] = [];

    let numDiagramXValues = 0;
    let numDuplicateXValues = 0;
    const originalEvalPointCount = evalPoints.length;

    for (let i = 0; i < originalEvalPointCount; i++) {
      const iEff = i + numDuplicateXValues;
      const location = evalPoints[iEff];

      // Check if location is not close to any regularIntervalLocations
      if (!regularIntervalLocations.some(value => Math.abs(value - location) <= 1e-4)) {
        // Equivalent to Debug.Assert(i != evalPoints.Count - 1);
        if (i === evalPoints.length - 1) {
          throw new Error("Unexpected: last evalPoint not in regularIntervalLocations");
        }

        const prevX = regularIntervalLocations[i - numDiagramXValues];
        const prevValue = relativeOffsets[iEff];
        const nextX = regularIntervalLocations[i - numDiagramXValues + 1];
        const nextValue = relativeOffsets[iEff + 1];

        const interpolated =
          prevX + (location - prevX) / (nextX - prevX) * (nextValue - prevValue);

        relativeOffsets.splice(iEff, 0, interpolated); // Insert at iEff

        numDiagramXValues++;
      }

      // Adapted: getValueAtLocation returns an object { leftValue, rightValue, isBetweenConsistantIntervals }
      const {
        leftValue: shearValOnLeft,
        rightValue: shearValOnRight,
        isBetweenConsistantIntervals: isBetweenIntervals
      } = getValueAtLocation(
        shearDiagram.intervals,
        { value: location, unit: LengthUnit.Meter },
        { value: 1, unit: LengthUnit.Inch }
      );
      const {
        leftValue: momValOnLeft,
        rightValue: momValOnRight,
        isBetweenConsistantIntervals: isBetweenMomIntervals
      } = getValueAtLocation(
        momentDiagram.intervals,
        { value: location, unit: LengthUnit.Meter },
        { value: 1, unit: LengthUnit.Inch }
      );

      shearValues.push(shearValOnLeft);
      momentValues.push(momValOnLeft);

      if (
        (isBetweenIntervals && Math.abs(shearValOnLeft - shearValOnRight) > 0.001) ||
        (isBetweenMomIntervals && Math.abs(momValOnLeft - momValOnRight) > 0.001)
      ) {
        // There is a jump in one of the graphs, need to add a point
        evalPoints.splice(iEff, 0, location);
        if (iEff === relativeOffsets.length) {
          relativeOffsets.push(relativeOffsets[relativeOffsets.length - 1]);
        } else {
          relativeOffsets.splice(iEff + 1, 0, relativeOffsets[iEff]);
        }
        shearValues.push(shearValOnRight);
        momentValues.push(momValOnRight);
        numDuplicateXValues++;
      }
    }

    console.log("evalPoints:", evalPoints);
    shearDiagramRef.current = <LineChartComponent
      xValues={evalPoints}
      yValues={shearValues}
      lineColor='rgba(255,199,0,1)'
      yAxisLabel='Shear (N)'
    />;

    momentDiagramRef.current = <LineChartComponent
      xValues={evalPoints}
      yValues={momentValues}
      lineColor='rgba(6,120,255,1)'
      yAxisLabel='Moment (Nm)'
    />;

    deflectionDiagramRef.current = <LineChartComponent
      xValues={evalPoints}
      yValues={relativeOffsets}
      lineColor='rgba(33, 166, 81,1)'
      yAxisLabel='Deflection (m)'
    />;

  }, [currentResultSet, element1dId, modelState]);

  return (
    <>
      {shearDiagramRef.current}
      {momentDiagramRef.current}
      {deflectionDiagramRef.current}
    </>
  );
}