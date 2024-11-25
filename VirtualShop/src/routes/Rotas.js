import { BrowserRouter, Route, Routes } from 'react-router-dom';
import Login from '../pages/TelaLogin';
import Cadastro from '../pages/TelaCadastro';
import Home from '../pages/Home';
import Inventario from '../pages/Inventario';
import Perfil from '../pages/Perfil';
import PrivateLayout from '../components/layout/PrivateLayout';
import PrivateRoute from '../components/PrivateRoute';

function Rotas() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/login" element={<Login />} />
        <Route path="/cadastro" element={<Cadastro />} />
        
        <Route element={<PrivateRoute><PrivateLayout /></PrivateRoute>}>
          <Route path="/home" element={<Home />} />
          <Route path="/inventario" element={<Inventario />} />
          <Route path="/perfil" element={<Perfil />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default Rotas;