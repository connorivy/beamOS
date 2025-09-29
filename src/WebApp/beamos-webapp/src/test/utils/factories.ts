// Common test data factories for consistent test data generation

export const createMockUser = (overrides = {}) => ({
  id: '1',
  name: 'John Doe',
  email: 'john@example.com',
  ...overrides,
})

export const createMockProduct = (overrides = {}) => ({
  id: '1',
  name: 'Sample Product',
  price: 99.99,
  description: 'A sample product for testing',
  ...overrides,
})

// Add more factory functions as needed for your domain objects