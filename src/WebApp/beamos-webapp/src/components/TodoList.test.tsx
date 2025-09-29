import { describe, it, expect } from 'vitest'
import { render, screen } from '../test/utils/test-utils'
import userEvent from '@testing-library/user-event'
import { TodoList } from './TodoList'

describe('TodoList Component', () => {
  it('should render empty todo list with add input', () => {
    render(<TodoList />)
    
    expect(screen.getByPlaceholderText(/add a new todo/i)).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /add/i })).toBeInTheDocument()
    expect(screen.getByText(/no todos yet/i)).toBeInTheDocument()
  })

  it('should add a new todo when form is submitted', async () => {
    const user = userEvent.setup()
    render(<TodoList />)
    
    const input = screen.getByPlaceholderText(/add a new todo/i)
    const addButton = screen.getByRole('button', { name: /add/i })
    
    await user.type(input, 'Learn React Testing')
    await user.click(addButton)
    
    expect(screen.getByText('Learn React Testing')).toBeInTheDocument()
    expect(screen.queryByText(/no todos yet/i)).not.toBeInTheDocument()
    expect(input).toHaveValue('')
  })

  it('should not add empty todo', async () => {
    const user = userEvent.setup()
    render(<TodoList />)
    
    const addButton = screen.getByRole('button', { name: /add/i })
    await user.click(addButton)
    
    expect(screen.getByText(/no todos yet/i)).toBeInTheDocument()
  })

  it('should mark todo as completed when checkbox is clicked', async () => {
    const user = userEvent.setup()
    render(<TodoList />)
    
    // Add a todo
    const input = screen.getByPlaceholderText(/add a new todo/i)
    await user.type(input, 'Test todo')
    await user.click(screen.getByRole('button', { name: /add/i }))
    
    // Click checkbox to complete
    const checkbox = screen.getByRole('checkbox')
    await user.click(checkbox)
    
    expect(checkbox).toBeChecked()
    expect(screen.getByText('Test todo')).toHaveStyle('text-decoration: line-through')
  })

  it('should delete todo when delete button is clicked', async () => {
    const user = userEvent.setup()
    render(<TodoList />)
    
    // Add a todo
    const input = screen.getByPlaceholderText(/add a new todo/i)
    await user.type(input, 'Todo to delete')
    await user.click(screen.getByRole('button', { name: /add/i }))
    
    // Delete the todo
    const deleteButton = screen.getByRole('button', { name: /delete/i })
    await user.click(deleteButton)
    
    expect(screen.queryByText('Todo to delete')).not.toBeInTheDocument()
    expect(screen.getByText(/no todos yet/i)).toBeInTheDocument()
  })

  it('should show todo count', async () => {
    const user = userEvent.setup()
    render(<TodoList />)
    
    expect(screen.getByText('Todos: 0')).toBeInTheDocument()
    
    // Add two todos
    const input = screen.getByPlaceholderText(/add a new todo/i)
    const addButton = screen.getByRole('button', { name: /add/i })
    
    await user.type(input, 'First todo')
    await user.click(addButton)
    
    await user.type(input, 'Second todo')
    await user.click(addButton)
    
    expect(screen.getByText('Todos: 2')).toBeInTheDocument()
  })
})