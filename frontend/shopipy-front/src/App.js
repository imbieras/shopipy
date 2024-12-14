// App.js
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { useState } from 'react';
import { AuthLayout } from './components/layouts/AuthLayout';
import LoginPage from "./components/auth/LoginPage";

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false)

  const handleLogin = async (email, password) => {
    // Login logic
    setIsAuthenticated(true)
  }

  return (
    <BrowserRouter>
      <div className="App">
        {/* <Navbar /> */}
        <Routes>
          {/* <Route 
            path="/" 
            element={isAuthenticated ? <HomePage /> : <Navigate to="/login" />} 
          /> */}
          <Route 
            path="/" 
            element={
              !isAuthenticated ? (
                <AuthLayout>
                    <LoginPage onLogin={handleLogin} />
                </AuthLayout>
              ) : (
                <Navigate to="/home" />
              )
            } 
          />
        </Routes>
      </div>
    </BrowserRouter>
  )
}

export default App;
