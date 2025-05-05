import React from 'react'
import './index.css' // Ensure this import is present for Tailwind CSS

function App() {
  return (
    <div className="min-h-screen bg-gray-100 p-8">
      <h1 className="text-3xl font-bold text-blue-600 mb-4">
        Welcome to React + Vite + Tailwind
      </h1>
      <button className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600">
        Click Me
      </button>
    </div>
  )
}

export default App