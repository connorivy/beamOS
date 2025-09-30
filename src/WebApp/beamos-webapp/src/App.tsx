import './App.css'
import * as React from 'react'
import { Counter, TodoList } from './components'
import { styled } from '@mui/material/styles'
import Toolbar from '@mui/material/Toolbar'

const StyledToolbar = styled(Toolbar)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  backgroundColor: theme.palette.primary.main,
  color: theme.palette.common.white,
  padding: theme.spacing(2),
  borderRadius: theme.shape.borderRadius,
}));

function App() {
  const [counterValue, setCounterValue] = React.useState(5) // Initialize to match Counter's initial value

  return (
    <>
      <h1 className='text-red-500'>BeamOS React TDD Demo</h1>
      
      <div style={{ display: 'flex', gap: '2rem', flexWrap: 'wrap' }}>
        <div style={{ flex: 1, minWidth: '300px' }}>
          <h2>Counter Component</h2>
          <Counter 
            initialCount={5} 
            onCountChange={setCounterValue}
          />
          <p>Current counter value: {counterValue}</p>
        </div>
        
        <div style={{ flex: 1, minWidth: '300px' }}>
          <h2>Todo List Component</h2>
          <TodoList />
        </div>
      </div>
      
      <div style={{ marginTop: '2rem', padding: '1rem', backgroundColor: '#f5f5f5', borderRadius: '8px' }}>
        <h3>🧪 TDD Setup Complete!</h3>
        <p>
          This project is now configured for Test-Driven Development. 
          Check the <code>TESTING.md</code> file for a comprehensive guide.
        </p>
        <p>
          <strong>Available test commands:</strong>
        </p>
        <ul>
          <li><code>npm test</code> - Run tests in watch mode</li>
          <li><code>npm run test:ui</code> - Run tests with visual UI</li>
          <li><code>npm run test:coverage</code> - Run tests with coverage report</li>
        </ul>
      </div>
    </>
  )
}

export default App
