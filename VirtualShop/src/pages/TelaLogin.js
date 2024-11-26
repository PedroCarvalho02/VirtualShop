import { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import userService from "../services/userService";
import "../styles/Login.css";

const Login = () => {
  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    const urlParams = new URLSearchParams(window.location.search);
    const token = urlParams.get("token");

    if (token) {
      localStorage.setItem("token", token);
      navigate("/home", { replace: true });
    }
  }, [navigate]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await userService.login({
        Email: email,
        Senha: senha,
      });

      const { token } = response;
      localStorage.setItem("token", token);
      navigate("/home");
    } catch (error) {
      alert(error.response?.data || "Email ou senha incorretos.");
      console.error(error);
    }
  };

  const handleGoogleLogin = () => {
    window.location.href = "http://localhost:5000/api/auth/google-login";
  };

  return (
    <div className="container d-flex justify-content-center align-items-center min-vh-100">
      <div className="row text-center col-12 col-md-6">
        <h1>Login</h1>
        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label htmlFor="email" className="form-label">
              Email:
            </label>
            <input
              type="email"
              id="email"
              name="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              className="form-control"
            />
          </div>
          <div className="mb-3">
            <label htmlFor="senha" className="form-label">
              Senha:
            </label>
            <input
              type="password"
              id="senha"
              name="senha"
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
              required
              className="form-control"
            />
          </div>
          <button type="submit" className="btn btn-primary w-100">
            Entrar
          </button>
        </form>
        <button onClick={handleGoogleLogin} className="btn btn-danger w-100 mt-3">
          Entrar com Google
        </button>
        <p>
          <Link to="/cadastro">Cadastre-se aqui</Link>
        </p>
      </div>
    </div>
  );
};

export default Login;