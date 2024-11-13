// src/routes/Rotas.js
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import Login from '../pages/TelaLogin';
import Cadastro from '../pages/TelaCadastro';
import Home from '../pages/Home';
import Inventario from '../pages/Inventario';
import PrivateLayout from '../components/layout/PrivateLayout';

function Rotas() {
  return (
    <BrowserRouter>
      <Routes>
       
        <Route path="/" element={<Login />} />
        <Route path="/login" element={<Login />} />
        <Route path="/cadastro" element={<Cadastro />} />
        
        <Route element={<PrivateLayout />}>
          <Route path="/home" element={<Home />} />
          <Route path="/inventario" element={<Inventario />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default Rotas;