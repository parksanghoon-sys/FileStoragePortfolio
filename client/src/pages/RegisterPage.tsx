import React, { useState } from 'react';
import { TextField, Button, Container, Paper, Typography, Box, Stack, Alert } from '@mui/material';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

function RegisterPage() {
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [message, setMessage] = useState('');
  const navigate = useNavigate();
  const { register } = useAuth();

  const handleRegister = async () => {
    setMessage(''); // Clear previous messages
    const success = await register(username, email, password);
    if (success) {
      setMessage('Registration successful! You can now log in.');
      navigate('/login');
    } else {
      setMessage('Registration failed: User with this email already exists or invalid data.');
    }
  };

  return (
    <Container maxWidth="xs" sx={{ mt: 8, height: '100vh', display: 'flex', flexDirection: 'column', justifyContent: 'center', alignItems: 'center' }}>
      <Paper elevation={3} sx={{ p: 4, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
        <Typography variant="h5" component="h1" gutterBottom>
          Register
        </Typography>
        <Box component="form" sx={{ mt: 1 }}>
          <TextField
            margin="normal"
            required
            fullWidth
            id="username"
            label="Username"
            name="username"
            autoComplete="username"
            autoFocus
            value={username}
            onChange={(e) => setUsername(e.target.value)}
          />
          <TextField
            margin="normal"
            required
            fullWidth
            id="email"
            label="Email Address"
            name="email"
            autoComplete="email"
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
            autoComplete="new-password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
          <Button
            type="button"
            fullWidth
            variant="contained"
            sx={{ mt: 3, mb: 2 }}
            onClick={handleRegister}
          >
            Sign Up
          </Button>
          <Stack direction="row" justifyContent="flex-end">
            <Link to="/login" style={{ textDecoration: 'none' }}>
              <Button variant="text">Already have an account? Sign In</Button>
            </Link>
          </Stack>
        </Box>
        {message && <Alert severity={message.includes('failed') ? 'error' : 'success'} sx={{ mt: 2 }}>{message}</Alert>}
      </Paper>
    </Container>
  );
}

export default RegisterPage;