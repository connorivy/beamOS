// Integration test example with multiple components
import { describe, it, expect } from 'vitest'
import { render, screen } from '../test/utils/test-utils'
import userEvent from '@testing-library/user-event'
import { Counter } from '../components/Counter'
import { TodoList } from '../components/TodoList'

describe('Integration Tests', () => {
  it('should render multiple components together', () => {
    render(
      <div>
        <Counter initialCount={5} />
        <TodoList />
      </div>
    )
    
    expect(screen.getByText('Count: 5')).toBeInTheDocument()
    expect(screen.getByPlaceholderText(/add a new todo/i)).toBeInTheDocument()
  })

  it('should handle complex user interactions', async () => {
    const user = userEvent.setup()
    
    render(
      <div>
        <Counter />
        <TodoList />
      </div>
    )
    
    // Interact with counter
    const incrementButton = screen.getByRole('button', { name: /increment/i })
    await user.click(incrementButton)
    await user.click(incrementButton)
    expect(screen.getByText('Count: 2')).toBeInTheDocument()
    
    // Interact with todo list
    const todoInput = screen.getByPlaceholderText(/add a new todo/i)
    await user.type(todoInput, 'Test integration')
    await user.click(screen.getByRole('button', { name: /add/i }))
    expect(screen.getByText('Test integration')).toBeInTheDocument()
  })
})