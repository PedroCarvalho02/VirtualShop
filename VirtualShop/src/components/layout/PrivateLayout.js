// src/components/layout/PrivateLayout.js
import { Outlet } from "react-router-dom";
import Navbar from "./Navbar";

const PrivateLayout = () => {
  const isAuthenticated = !!localStorage.getItem("user");

  // if (!isAuthenticated) {
  //     window.location.href = '/';
  //     return null;
  // }

  return (
    <>
      <Navbar />
      <Outlet />
    </>
  );
};

export default PrivateLayout;
