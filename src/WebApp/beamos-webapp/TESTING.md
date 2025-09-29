# React TDD Testing Guide

This project is set up for Test-Driven Development (TDD) with React using Vitest and React Testing Library.

## 🧪 Testing Stack

- **Vitest**: Fast unit test framework (Jest alternative for Vite)
- **React Testing Library**: Testing utilities focused on user behavior
- **@testing-library/jest-dom**: Custom Jest matchers for DOM testing
- **@testing-library/user-event**: Advanced user interaction simulation
- **jsdom**: DOM implementation for Node.js

## 📁 Project Structure

```
src/
├── components/           # React components
│   ├── Counter.tsx      # Example component
│   ├── Counter.test.tsx # Component tests
│   ├── TodoList.tsx     # Example TDD component
│   └── TodoList.test.tsx
├── test/                # Testing configuration and utilities
│   ├── setup.ts         # Global test setup
│   ├── integration.test.tsx # Integration tests
│   ├── utils/
│   │   ├── test-utils.tsx   # Custom render function
│   │   └── factories.ts     # Test data factories
│   └── mocks/
│       └── handlers.ts      # Mock API handlers
```

## 🚀 Available Scripts

```bash
# Run tests in watch mode
npm run test

# Run tests with UI
npm run test:ui

# Run tests with coverage
npm run test:coverage

# Run tests in watch mode (explicit)
npm run test:watch
```

## 🔄 TDD Workflow

### 1. Red - Write a Failing Test
Write a test for the functionality you want to implement:

```typescript
// Component.test.tsx
import { describe, it, expect } from 'vitest'
import { render, screen } from '../test/utils/test-utils'
import { MyComponent } from './MyComponent'

describe('MyComponent', () => {
  it('should render hello message', () => {
    render(<MyComponent name="World" />)
    expect(screen.getByText('Hello, World!')).toBeInTheDocument()
  })
})
```

### 2. Green - Make the Test Pass
Implement the minimal code to make the test pass:

```typescript
// MyComponent.tsx
interface MyComponentProps {
  name: string
}

export const MyComponent = ({ name }: MyComponentProps) => {
  return <div>Hello, {name}!</div>
}
```

### 3. Refactor - Improve the Code
Refactor while keeping tests green:

```typescript
// Improved version with better styling/structure
export const MyComponent = ({ name }: MyComponentProps) => {
  return (
    <div className="greeting">
      <h1>Hello, {name}!</h1>
    </div>
  )
}
```

## 🛠 Testing Patterns

### Component Testing
```typescript
import { render, screen } from '../test/utils/test-utils'
import userEvent from '@testing-library/user-event'

describe('Button Component', () => {
  it('should call onClick when clicked', async () => {
    const user = userEvent.setup()
    const mockOnClick = vi.fn()
    
    render(<Button onClick={mockOnClick}>Click me</Button>)
    
    await user.click(screen.getByRole('button'))
    expect(mockOnClick).toHaveBeenCalledOnce()
  })
})
```

### Form Testing
```typescript
it('should submit form with correct data', async () => {
  const user = userEvent.setup()
  const mockOnSubmit = vi.fn()
  
  render(<ContactForm onSubmit={mockOnSubmit} />)
  
  await user.type(screen.getByLabelText(/name/i), 'John Doe')
  await user.type(screen.getByLabelText(/email/i), 'john@example.com')
  await user.click(screen.getByRole('button', { name: /submit/i }))
  
  expect(mockOnSubmit).toHaveBeenCalledWith({
    name: 'John Doe',
    email: 'john@example.com'
  })
})
```

### Async Testing
```typescript
it('should load data after mount', async () => {
  render(<DataComponent />)
  
  expect(screen.getByText(/loading/i)).toBeInTheDocument()
  
  await waitFor(() => {
    expect(screen.getByText(/data loaded/i)).toBeInTheDocument()
  })
})
```

## 🧩 Custom Test Utilities

### Test Data Factories
Use factories for consistent test data:

```typescript
// factories.ts
export const createMockUser = (overrides = {}) => ({
  id: '1',
  name: 'John Doe',
  email: 'john@example.com',
  ...overrides,
})

// In tests
const user = createMockUser({ name: 'Jane Smith' })
```

### Custom Render
The custom render function in `test-utils.tsx` can be extended with providers:

```typescript
const customRender = (ui: ReactElement, options?: RenderOptions) => {
  return render(ui, {
    wrapper: ({ children }) => (
      <BrowserRouter>
        <ThemeProvider theme={testTheme}>
          {children}
        </ThemeProvider>
      </BrowserRouter>
    ),
    ...options,
  })
}
```

## 📋 Best Practices

1. **Write tests first** - Follow TDD principles
2. **Test behavior, not implementation** - Focus on what users see and do
3. **Use semantic queries** - Prefer `getByRole`, `getByLabelText`, etc.
4. **Keep tests simple** - One concept per test
5. **Use descriptive test names** - Clearly state what is being tested
6. **Mock external dependencies** - Keep tests isolated
7. **Test edge cases** - Error states, empty states, loading states

## 🔍 Debugging Tests

### Visual Debugging
```typescript
import { screen } from '@testing-library/react'

// See what's rendered
screen.debug()

// See specific element
screen.debug(screen.getByRole('button'))
```

### Query Debugging
```typescript
// See available queries
screen.getByRole('') // Will show available roles in error message
```

### Running Single Tests
```bash
# Run specific test file
npm test Counter.test.tsx

# Run tests matching pattern
npm test -- --grep "should increment"
```

## 📚 Examples

Check out the example components:
- `Counter.tsx` - Simple state management with callbacks
- `TodoList.tsx` - Form handling, lists, and CRUD operations
- `integration.test.tsx` - Testing multiple components together

## 🎯 Next Steps

1. Add more complex components following TDD
2. Set up MSW for API mocking
3. Add E2E tests with Playwright/Cypress
4. Set up coverage reporting
5. Add visual regression testing

Happy Testing! 🎉