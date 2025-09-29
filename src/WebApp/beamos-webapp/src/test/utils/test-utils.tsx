import { render } from '@testing-library/react'
import type { RenderOptions } from '@testing-library/react'
import type { ReactElement } from 'react'

// Custom render function that includes providers if needed
const customRender = (
  ui: ReactElement,
  options?: Omit<RenderOptions, 'wrapper'>,
) => {
  return render(ui, {
    // Add any providers here (e.g., Redux store, Router, Theme providers)
    ...options,
  })
}

// Re-export everything from React Testing Library
export * from '@testing-library/react'
export { customRender as render }