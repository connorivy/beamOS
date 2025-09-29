# TDD Workflow Example

This file demonstrates a complete TDD workflow by building a simple User Profile component.

## Step 1: Write a Failing Test (RED)

```typescript
// UserProfile.test.tsx
import { describe, it, expect, vi } from 'vitest'
import { render, screen } from '../test/utils/test-utils'
import userEvent from '@testing-library/user-event'
import { UserProfile } from './UserProfile'

describe('UserProfile Component', () => {
  const mockUser = {
    id: '1',
    name: 'John Doe',
    email: 'john@example.com',
    bio: 'Software developer'
  }

  it('should display user information', () => {
    render(<UserProfile user={mockUser} />)
    
    expect(screen.getByText('John Doe')).toBeInTheDocument()
    expect(screen.getByText('john@example.com')).toBeInTheDocument()
    expect(screen.getByText('Software developer')).toBeInTheDocument()
  })

  it('should show edit form when edit button is clicked', async () => {
    const user = userEvent.setup()
    render(<UserProfile user={mockUser} />)
    
    const editButton = screen.getByRole('button', { name: /edit/i })
    await user.click(editButton)
    
    expect(screen.getByDisplayValue('John Doe')).toBeInTheDocument()
    expect(screen.getByDisplayValue('john@example.com')).toBeInTheDocument()
    expect(screen.getByDisplayValue('Software developer')).toBeInTheDocument()
  })

  it('should call onSave when form is submitted', async () => {
    const user = userEvent.setup()
    const mockOnSave = vi.fn()
    render(<UserProfile user={mockUser} onSave={mockOnSave} />)
    
    // Enter edit mode
    await user.click(screen.getByRole('button', { name: /edit/i }))
    
    // Modify the name
    const nameInput = screen.getByDisplayValue('John Doe')
    await user.clear(nameInput)
    await user.type(nameInput, 'Jane Smith')
    
    // Submit form
    await user.click(screen.getByRole('button', { name: /save/i }))
    
    expect(mockOnSave).toHaveBeenCalledWith({
      ...mockUser,
      name: 'Jane Smith'
    })
  })
})
```

## Step 2: Make the Test Pass (GREEN)

```typescript
// UserProfile.tsx
import { useState } from 'react'

interface User {
  id: string
  name: string
  email: string
  bio: string
}

interface UserProfileProps {
  user: User
  onSave?: (user: User) => void
}

export const UserProfile = ({ user, onSave }: UserProfileProps) => {
  const [isEditing, setIsEditing] = useState(false)
  const [formData, setFormData] = useState(user)

  const handleEdit = () => {
    setIsEditing(true)
    setFormData(user)
  }

  const handleSave = () => {
    onSave?.(formData)
    setIsEditing(false)
  }

  const handleCancel = () => {
    setFormData(user)
    setIsEditing(false)
  }

  if (isEditing) {
    return (
      <div>
        <h2>Edit Profile</h2>
        <form>
          <div>
            <label htmlFor="name">Name:</label>
            <input
              id="name"
              type="text"
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            />
          </div>
          <div>
            <label htmlFor="email">Email:</label>
            <input
              id="email"
              type="email"
              value={formData.email}
              onChange={(e) => setFormData({ ...formData, email: e.target.value })}
            />
          </div>
          <div>
            <label htmlFor="bio">Bio:</label>
            <textarea
              id="bio"
              value={formData.bio}
              onChange={(e) => setFormData({ ...formData, bio: e.target.value })}
            />
          </div>
          <button type="button" onClick={handleSave}>Save</button>
          <button type="button" onClick={handleCancel}>Cancel</button>
        </form>
      </div>
    )
  }

  return (
    <div>
      <h2>{user.name}</h2>
      <p>Email: {user.email}</p>
      <p>Bio: {user.bio}</p>
      <button onClick={handleEdit}>Edit</button>
    </div>
  )
}
```

## Step 3: Refactor (REFACTOR)

```typescript
// UserProfile.tsx - Refactored version
import { useState } from 'react'

interface User {
  id: string
  name: string
  email: string
  bio: string
}

interface UserProfileProps {
  user: User
  onSave?: (user: User) => void
}

// Extract form component for better separation of concerns
const UserForm = ({ 
  user, 
  onSave, 
  onCancel 
}: { 
  user: User
  onSave: (user: User) => void
  onCancel: () => void 
}) => {
  const [formData, setFormData] = useState(user)

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    onSave(formData)
  }

  const updateField = (field: keyof User, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }))
  }

  return (
    <div>
      <h2>Edit Profile</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="name">Name:</label>
          <input
            id="name"
            type="text"
            value={formData.name}
            onChange={(e) => updateField('name', e.target.value)}
            required
          />
        </div>
        <div>
          <label htmlFor="email">Email:</label>
          <input
            id="email"
            type="email"
            value={formData.email}
            onChange={(e) => updateField('email', e.target.value)}
            required
          />
        </div>
        <div>
          <label htmlFor="bio">Bio:</label>
          <textarea
            id="bio"
            value={formData.bio}
            onChange={(e) => updateField('bio', e.target.value)}
            rows={4}
          />
        </div>
        <div>
          <button type="submit">Save</button>
          <button type="button" onClick={onCancel}>Cancel</button>
        </div>
      </form>
    </div>
  )
}

const UserDisplay = ({ 
  user, 
  onEdit 
}: { 
  user: User
  onEdit: () => void 
}) => (
  <div>
    <h2>{user.name}</h2>
    <p>Email: {user.email}</p>
    <p>Bio: {user.bio}</p>
    <button onClick={onEdit}>Edit</button>
  </div>
)

export const UserProfile = ({ user, onSave }: UserProfileProps) => {
  const [isEditing, setIsEditing] = useState(false)

  const handleSave = (updatedUser: User) => {
    onSave?.(updatedUser)
    setIsEditing(false)
  }

  const handleCancel = () => {
    setIsEditing(false)
  }

  return isEditing ? (
    <UserForm 
      user={user} 
      onSave={handleSave} 
      onCancel={handleCancel} 
    />
  ) : (
    <UserDisplay 
      user={user} 
      onEdit={() => setIsEditing(true)} 
    />
  )
}
```

## TDD Benefits Demonstrated

1. **Confidence**: All functionality is tested before implementation
2. **Design**: Tests drive better component API design
3. **Documentation**: Tests serve as living documentation
4. **Refactoring**: Safe to refactor with comprehensive test coverage
5. **Debugging**: Easier to identify issues with granular tests

## Next Steps

1. Add validation tests
2. Test error handling
3. Add accessibility tests
4. Test with different user types
5. Add integration tests with API calls