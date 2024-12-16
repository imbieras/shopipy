import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { AuthLayout } from './core/layouts/AuthLayout';
import LoginPage from "./core/auth/LoginPage";
import { authService } from './core/auth/services/AuthService';
import { useUser } from './hooks/useUser';
import { jwtDecode } from 'jwt-decode';
import Navbar from './core/ui/Navbar';
import Services from './core/serviceManagement/page';
import Appointments from './core/appointmentManagement/Appointments';
import { useQueryClient } from '@tanstack/react-query';
import { useQuery } from '@tanstack/react-query';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const { fetchUser, clearUser } = useUser();
  const queryClient = useQueryClient();

  const checkAuthentication = async () => {
    const token = authService.getToken();
    if (!token) {
      setIsAuthenticated(false);
      return;
    }

    try {
      const decodedToken = jwtDecode(token);
      if (decodedToken.exp * 1000 > Date.now()) {
        if (decodedToken.sub) {
          await fetchUser(decodedToken.sub);
        }
        setIsAuthenticated(true);
      } else {
        handleLogout();
      }
    } catch (error) {
      console.error("Error decoding token:", error);
      handleLogout();
    }
  };

  useEffect(() => {
    checkAuthentication();
  }, []);

  useQuery({
    queryKey: ['auth'],
    queryFn: async () => {
      const token = authService.getToken();
      if (!token) return null;

      try {
        const decodedToken = jwtDecode(token);
        if (decodedToken.sub) {
          await fetchUser(decodedToken.sub);
        }
        return decodedToken;
      } catch (error) {
        handleLogout();
        return null;
      }
    },
    enabled: !!authService.getToken(),
    staleTime: Infinity, 
  });

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
    clearUser();
    queryClient.invalidateQueries(['auth']);
    queryClient.clear();
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
                <Navigate to="/services"/>
              )
            }
          />
          <Route
            path="/services"
            element={
              isAuthenticated ? (
                <Services/>
              ) : (
                <Navigate to="/" />
              )
            }
          />
          <Route
            path="/appointments"
            element={
              isAuthenticated ? (
                <Appointments/>
              ) : (
                <Navigate to="/"/>
              )
            }
          />
        </Routes>
      </div>
    </BrowserRouter>
  );
}

export default App;
