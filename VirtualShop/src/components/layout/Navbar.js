import { Link, useNavigate } from "react-router-dom";
import userService from "../../services/userService";

const Navbar = () => {
  const navigate = useNavigate();
  const token = localStorage.getItem("token");

  const handleLogout = async () => {
    try {
      await userService.logout(token);
      localStorage.removeItem("token");
      navigate("/login");
    } catch (error) {
      alert("Erro ao realizar logout.");
      console.error(error);
    }
  };

  return (
    <nav className="navbar navbar-expand-lg navbar-light bg-light">
      <div className="container-fluid">
        <Link className="navbar-brand" to="/home">VirtualShop</Link>
        <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
          <span className="navbar-toggler-icon"></span>
        </button>
        <div className="collapse navbar-collapse" id="navbarNav">
          <ul className="navbar-nav ms-auto">
            <li className="nav-item">
              <Link className="nav-link" to="/home">Home</Link>
            </li>
            <li className="nav-item">
              <Link className="nav-link" to="/inventario">Inventário</Link>
            </li>
            <li className="nav-item">
              <Link className="nav-link" to="/perfil">Perfil</Link>
            </li>
            <li className="nav-item">
              <button
                className="btn btn-link nav-link"
                onClick={handleLogout}
              >
                Sair
              </button>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;