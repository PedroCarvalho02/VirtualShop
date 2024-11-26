import { useEffect, useState } from "react";
import productService from "../services/productService";
import "../styles/Home.css";

const Home = () => {
  const [produtos, setProdutos] = useState([]);

  useEffect(() => {
    const fetchProdutos = async () => {
      try {
        const data = await productService.getAllProducts();
        console.log("Produtos recebidos:", data); 
        setProdutos(Array.isArray(data) ? data : []);
      } catch (error) {
        console.error("Erro ao buscar produtos:", error);
        setProdutos([]);
      }
    };

    fetchProdutos();
  }, []);

  return (
    <div className="container mt-4">
      <h1>Home</h1>
      <p>Bem-vindo ao VirtualShop!</p>
      <div className="row">
        {produtos.length > 0 ? (
          produtos.map((produto) => (
            <div key={produto.id} className="col-md-4 mb-4">
              <div className="card">
                <img src={produto.imageUrl} className="card-img-top" alt={produto.nome} />
                <div className="card-body">
                  <h5 className="card-title">{produto.nome}</h5>
                  <p className="card-text">Preço: R${produto.preco}</p>
                </div>
              </div>
            </div>
          ))
        ) : (
          <p>Nenhum produto encontrado.</p>
        )}
      </div>
    </div>
  );
};

export default Home;