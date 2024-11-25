import { useEffect, useState } from "react";
import userService from "../services/userService";

const Perfil = () => {
  const [perfil, setPerfil] = useState(null);
  const token = localStorage.getItem("token");

  useEffect(() => {
    const fetchPerfil = async () => {
      try {
        const data = await userService.getProfile(token);
        setPerfil(data);
      } catch (error) {
        alert("Erro ao obter perfil.");
        console.error(error);
      }
    };

    if (token) {
      fetchPerfil();
    }
  }, [token]);

  if (!perfil) {
    return <div className="container"><p>Carregando perfil...</p></div>;
  }

  return (
    <div className="container mt-4">
      <h1>Perfil do Usuário</h1>
      <p><strong>Nome:</strong> {perfil.NomeUsuario}</p>
      <p><strong>Email:</strong> {perfil.Email}</p>
      <p><strong>CPF:</strong> {perfil.Cpf}</p>
      <p><strong>Administrador:</strong> {perfil.IsAdmin ? "Sim" : "Não"}</p>
    </div>
  );
};

export default Perfil;