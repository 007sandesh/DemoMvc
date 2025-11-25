import { useState } from 'react';

function Login({ onLogin, onSwitchToRegister }) {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');

    const handleSubmit = (e) => {
        e.preventDefault();
        setError('');

        fetch('https://localhost:7015/api/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password })
        })
            .then(res => {
                if (!res.ok) throw new Error('Invalid username or password');
                return res.json();
            })
            .then(data => {
                // Save token and user info
                localStorage.setItem('token', data.token);
                localStorage.setItem('role', data.role);
                localStorage.setItem('username', data.username);

                // Notify App component
                onLogin(data);
            })
            .catch(err => setError(err.message));
    };

    return (
        <div className="card" style={{ maxWidth: '400px', margin: '50px auto' }}>
            <h2>🔐 Login</h2>
            {error && <div style={{ color: 'red', marginBottom: '10px' }}>{error}</div>}

            <form onSubmit={handleSubmit} className="form-grid">
                <input
                    type="text"
                    placeholder="Username"
                    value={username}
                    onChange={e => setUsername(e.target.value)}
                    required
                />
                <input
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={e => setPassword(e.target.value)}
                    required
                />
                <button type="submit" className="primary-btn">Login</button>
            </form>
            <p style={{ marginTop: '15px', textAlign: 'center' }}>
                Don't have an account?{' '}
                <button
                    onClick={onSwitchToRegister}
                    style={{ background: 'none', border: 'none', color: '#4f46e5', cursor: 'pointer', textDecoration: 'underline' }}
                >
                    Register here
                </button>
            </p>
        </div>
    );
}

export default Login;
