import React, { useEffect, useState } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import userService from "../services/userService";

const PrivateRoute = ({ children, adminOnly }) => {
  const location = useLocation();
  const token = localStorage.getItem("token");
  const [isAdmin, setIsAdmin] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchPerfil = async () => {
      try {
        const perfil = await userService.getProfile(token);
        setIsAdmin(perfil.isAdmin);
      } catch (error) {
        console.error("Erro ao obter perfil:", error);
      } finally {
        setIsLoading(false);
      }
    };

    if (token) {
      fetchPerfil();
    } else {
      setIsLoading(false);
    }
  }, [token]);

  if (isLoading) {
    return <div className="container"><p>Carregando...</p></div>;
  }

  if (!token) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (adminOnly && !isAdmin) {
    return <Navigate to="/home" replace />;
  }

  return children;
};

export default PrivateRoute;  