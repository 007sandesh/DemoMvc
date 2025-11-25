import { useState } from 'react';

function Register({ onRegisterSuccess, onSwitchToLogin }) {
    const [formData, setFormData] = useState({
        username: '',
        email: '',
        password: '',
        role: 'Student' // Default role
    });
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        setError('');
        setSuccess('');

        fetch('https://localhost:7015/api/auth/register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(formData)
        })
            .then(async res => {
                if (!res.ok) {
                    const text = await res.text();
                    throw new Error(text || 'Registration failed');
                }
                return res.json();
            })
            .then(() => {
                setSuccess('Registration successful! You can now login.');
                setTimeout(() => {
                    onRegisterSuccess();
                }, 2000);
            })
            .catch(err => setError(err.message));
    };

    return (
        <div className="card" style={{ maxWidth: '400px', margin: '50px auto' }}>
            <h2>📝 Register</h2>
            {error && <div style={{ color: 'red', marginBottom: '10px' }}>{error}</div>}
            {success && <div style={{ color: 'green', marginBottom: '10px' }}>{success}</div>}

            <form onSubmit={handleSubmit} className="form-grid">
                <input
                    type="text"
                    name="username"
                    placeholder="Username"
                    value={formData.username}
                    onChange={handleChange}
                    required
                />
                <input
                    type="email"
                    name="email"
                    placeholder="Email"
                    value={formData.email}
                    onChange={handleChange}
                    required
                />
                <input
                    type="password"
                    name="password"
                    placeholder="Password"
                    value={formData.password}
                    onChange={handleChange}
                    required
                />
                <select
                    name="role"
                    value={formData.role}
                    onChange={handleChange}
                    style={{ padding: '10px', borderRadius: '8px', border: '1px solid #ddd' }}
                >
                    <option value="Student">Student</option>
                    <option value="Teacher">Teacher</option>
                    <option value="Admin">Admin</option>
                </select>
                <button type="submit" className="primary-btn">Register</button>
            </form>
            <p style={{ marginTop: '15px', textAlign: 'center' }}>
                Already have an account?{' '}
                <button
                    onClick={onSwitchToLogin}
                    style={{ background: 'none', border: 'none', color: '#4f46e5', cursor: 'pointer', textDecoration: 'underline' }}
                >
                    Login here
                </button>
            </p>
        </div>
    );
}

export default Register;
