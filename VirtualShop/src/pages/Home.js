import React from "react";
import { Link } from "react-router-dom";

const Home = () => {
  return (
    <div className="container mt-4">
      <h1>Home</h1>
      <p>Bem-vindo ao VirtualShop!</p>
      <Link to="/perfil" className="btn btn-primary">Ver Perfil</Link>
    </div>
  );
};

export default Home;