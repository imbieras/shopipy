// App.js
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { AuthLayout } from './components/layouts/AuthLayout';
import LoginPage from "./components/auth/LoginPage";
import { authService } from './components/auth/services/AuthService';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false)

  useEffect(() => {
    setIsAuthenticated(authService.isAuthenticated());
  }, [])

  const handleLogin = async (username, password) => {
    try {
        const response = await authService.login(username, password);
        setIsAuthenticated(true);
        console.log("Login successful:", response);
        return response;
    } catch (error) {
        console.error("Login failed:", error);
        setIsAuthenticated(false);
        throw error;
    }
};

  const handleLogout = () => {
    authService.logout();
    setIsAuthenticated(false);
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
