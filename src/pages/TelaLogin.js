// src/pages/TelaLogin.js
import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';

const Login = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const navigate = useNavigate();

    const handleSubmit = (e) => {
        e.preventDefault();
        const storedUser = localStorage.getItem('user');
        if (storedUser) {
            const { email: storedEmail, password: storedPassword } = JSON.parse(storedUser);
            if (email === storedEmail && password === storedPassword) {
                navigate('/home');
            } else {
                alert('Email ou senha incorretos.');
            }
        } else {
            alert('Nenhum usuário cadastrado.');
        }
    };

    return (
        <div>
            <h1>Login</h1>
            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="email">Email:</label>
                    <input
                        type="email"
                        id="email"
                        name="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label htmlFor="password">Senha:</label>
                    <input
                        type="password"
                        id="password"
                        name="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>
                <button type="submit">Entrar</button>
            </form>
            <p>
                <Link to="/cadastro">Cadastre-se aqui</Link>
            </p>
        </div>
    );
}

export default Login;