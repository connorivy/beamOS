import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '../test/utils/test-utils'
import { Counter } from './Counter'

describe('Counter Component', () => {
  it('should render with initial count of 0', () => {
    render(<Counter />)
    expect(screen.getByText('Count: 0')).toBeInTheDocument()
  })

  it('should render with custom initial count', () => {
    render(<Counter initialCount={5} />)
    expect(screen.getByText('Count: 5')).toBeInTheDocument()
  })

  it('should increment count when increment button is clicked', () => {
    render(<Counter />)
    const incrementButton = screen.getByRole('button', { name: /increment/i })
    
    fireEvent.click(incrementButton)
    expect(screen.getByText('Count: 1')).toBeInTheDocument()
  })

  it('should decrement count when decrement button is clicked', () => {
    render(<Counter initialCount={1} />)
    const decrementButton = screen.getByRole('button', { name: /decrement/i })
    
    fireEvent.click(decrementButton)
    expect(screen.getByText('Count: 0')).toBeInTheDocument()
  })

  it('should reset count to initial value when reset button is clicked', () => {
    render(<Counter initialCount={5} />)
    const incrementButton = screen.getByRole('button', { name: /increment/i })
    const resetButton = screen.getByRole('button', { name: /reset/i })
    
    // Change the count
    fireEvent.click(incrementButton)
    fireEvent.click(incrementButton)
    expect(screen.getByText('Count: 7')).toBeInTheDocument()
    
    // Reset should go back to initial value
    fireEvent.click(resetButton)
    expect(screen.getByText('Count: 5')).toBeInTheDocument()
  })

  it('should call onCountChange callback when count changes', () => {
    const mockOnCountChange = vi.fn()
    render(<Counter onCountChange={mockOnCountChange} />)
    
    const incrementButton = screen.getByRole('button', { name: /increment/i })
    fireEvent.click(incrementButton)
    
    expect(mockOnCountChange).toHaveBeenCalledWith(1)
  })
})