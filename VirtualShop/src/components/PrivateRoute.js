import React, { useEffect } from 'react';
import { Navigate, useLocation } from 'react-router-dom';

const PrivateRoute = ({ children }) => {
  const location = useLocation();
  const token = localStorage.getItem("token");
  const urlParams = new URLSearchParams(location.search);
  const tokenFromURL = urlParams.get("token");

  useEffect(() => {
    if (tokenFromURL) {
      localStorage.setItem("token", tokenFromURL);
      window.history.replaceState({}, document.title, "/home");
    }
  }, [tokenFromURL]);

  if (!token && !tokenFromURL) {
    return <Navigate to="/login" />;
  }

  return children;
};

export default PrivateRoute;