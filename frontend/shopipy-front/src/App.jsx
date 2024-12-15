import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { AuthLayout } from './core/layouts/AuthLayout';
import LoginPage from "./core/auth/LoginPage";
import { authService } from './core/auth/services/AuthService';
import Services from "./core/serviceManagement/Services";
import { useUser } from './hooks/useUser';
import { jwtDecode } from 'jwt-decode';
import Navbar from './core/ui/Navbar';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const { fetchUser, fetched } = useUser();

  useEffect(() => {
    const checkAuthAndFetchUser = async () => {
      const token = authService.getToken();
      if (token) {
        try {
          const decodedToken = jwtDecode(token);
          if (decodedToken.sub) {
            await fetchUser(decodedToken.sub);
          }
        } catch (error) {
          console.error("Error fetching user:", error);
        }
      }
      setIsAuthenticated(authService.isAuthenticated());
    };

    checkAuthAndFetchUser();
  }, [fetchUser]);

  const handleLogin = async (username, password) => {
    try {
      const response = await authService.login(username, password);
      const decodedToken = jwtDecode(response.token);
      if (decodedToken.id) {
        await fetchUser(decodedToken.id);
      }
      setIsAuthenticated(true);
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
  };

  return (
    <BrowserRouter>
      <div className="App">
      {isAuthenticated && <Navbar onLogout={handleLogout} />}
        <Routes>
          <Route
            path="/"
            element={
              !isAuthenticated ? (
                <AuthLayout>
                  <LoginPage onLogin={handleLogin} />
                </AuthLayout>
              ) : (
                <Navigate to="/services" />
              )
            }
          />
          <Route
            path="/services"
            element={
              isAuthenticated ? (
                <Services onLogout={handleLogout} />
              ) : (
                <Navigate to="/" />
              )
            }
          />
        </Routes>
      </div>
    </BrowserRouter>
  );
}

export default App;