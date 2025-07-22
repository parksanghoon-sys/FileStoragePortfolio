import React, { useState } from 'react';
import { TextField, Button, Container, Paper, Typography, Box, Stack, Alert } from '@mui/material';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [message, setMessage] = useState('');
  const navigate = useNavigate();
  const { login } = useAuth();

  const handleLogin = async () => {
    setMessage(''); // Clear previous messages
    const success = await login(email, password);
    if (success) {
      setMessage('Login successful!');
      navigate('/dashboard');
    } else {
      setMessage('Login failed: Invalid credentials.');
    }
  };

  return (
    <Container maxWidth="xs" sx={{ mt: 8, height: '100vh', display: 'flex', flexDirection: 'column', justifyContent: 'center', alignItems: 'center' }}>
      <Paper elevation={3} sx={{ p: 4, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
        <Typography variant="h5" component="h1" gutterBottom>
          Login
        </Typography>
        <Box component="form" sx={{ mt: 1 }}>
          <TextField
            margin="normal"
            required
            fullWidth
            id="email"
            label="Email Address"
            name="email"
            autoComplete="email"
            autoFocus
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
          <TextField
            margin="normal"
            required
            fullWidth
            name="password"
            label="Password"
            type="password"
            id="password"
            autoComplete="current-password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
          <Button
            type="button"
            fullWidth
            variant="contained"
            sx={{ mt: 3, mb: 2 }}
            onClick={handleLogin}
          >
            Sign In
          </Button>
          <Stack direction="row" justifyContent="flex-end">
            <Link to="/register" style={{ textDecoration: 'none' }}>
              <Button variant="text">Don't have an account? Sign Up</Button>
            </Link>
          </Stack>
        </Box>
        {message && <Alert severity={message.includes('failed') ? 'error' : 'success'} sx={{ mt: 2 }}>{message}</Alert>}
      </Paper>
    </Container>
  );
}

export default LoginPage;