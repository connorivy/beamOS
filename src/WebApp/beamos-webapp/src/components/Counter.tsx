import { useState } from 'react'

interface CounterProps {
  initialCount?: number
  onCountChange?: (count: number) => void
}

export const Counter = ({ initialCount = 0, onCountChange }: CounterProps) => {
  const [count, setCount] = useState(initialCount)

  const handleIncrement = () => {
    const newCount = count + 1
    setCount(newCount)
    onCountChange?.(newCount)
  }

  const handleDecrement = () => {
    const newCount = count - 1
    setCount(newCount)
    onCountChange?.(newCount)
  }

  const handleReset = () => {
    setCount(initialCount)
    onCountChange?.(initialCount)
  }

  return (
    <div>
      <p>Count: {count}</p>
      <button onClick={handleIncrement}>Increment</button>
      <button onClick={handleDecrement}>Decrement</button>
      <button onClick={handleReset}>Reset</button>
    </div>
  )
}