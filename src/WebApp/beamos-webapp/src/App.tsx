import { useState } from 'react'
import './App.css'
import { Counter, TodoList } from './components'

function App() {
  const [counterValue, setCounterValue] = useState(5) // Initialize to match Counter's initial value

  return (
    <>
      <h1>BeamOS React TDD Demo</h1>
      
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
