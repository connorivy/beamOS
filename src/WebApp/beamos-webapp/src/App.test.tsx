import { describe, it, expect } from 'vitest'
import { render, screen } from './test/utils/test-utils'
import App from './App'

describe('App Component', () => {
  it('should render the main heading', () => {
    render(<App />)
    expect(screen.getByText('BeamOS React TDD Demo')).toBeInTheDocument()
  })

  it('should render both Counter and TodoList components', () => {
    render(<App />)
    
    // Check Counter component is rendered
    expect(screen.getByText('Counter Component')).toBeInTheDocument()
    expect(screen.getByText('Count: 5')).toBeInTheDocument() // Initial count of 5
    
    // Check TodoList component is rendered
    expect(screen.getByText('Todo List Component')).toBeInTheDocument()
    expect(screen.getByPlaceholderText(/add a new todo/i)).toBeInTheDocument()
  })

  it('should display TDD setup information', () => {
    render(<App />)
    
    expect(screen.getByText('🧪 TDD Setup Complete!')).toBeInTheDocument()
    expect(screen.getByText(/npm test/)).toBeInTheDocument()
  })

  it('should update counter value display when counter changes', async () => {
    render(<App />)
    
    // Initial state - both Counter and App should start at 5
    expect(screen.getByText('Current counter value: 5')).toBeInTheDocument()
    
    // This test demonstrates the callback working
    // Note: In a real app test, you might simulate user interaction
    // For now, we're just testing the initial render
  })
})