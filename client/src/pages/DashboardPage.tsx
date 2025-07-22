import React, { useState, useEffect } from 'react';
import { 
  AppBar, 
  Toolbar, 
  Typography, 
  Button, 
  Box, 
  Drawer, 
  List, 
  ListItem, 
  ListItemText, 
  CssBaseline, 
  Divider, 
  Container, 
  Paper, 
  Stack, 
  Alert, 
  TextField, 
  Dialog, 
  DialogActions, 
  DialogContent, 
  DialogContentText, 
  DialogTitle 
} from '@mui/material';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';

const drawerWidth = 240;

interface FileDto {
  id: number;
  fileName: string;
  fileSize: number;
  contentType: string;
  createdAt: string;
}

interface SharedFileDto {
  id: number;
  fileName: string;
  sharedBy: string;
  sharedAt: string;
  shareToken: string;
  isAccepted: boolean;
}

function DashboardPage() {
  const { user, logout, token } = useAuth();
  const navigate = useNavigate();
  const [selectedMenu, setSelectedMenu] = useState('my-files');
  const [myFiles, setMyFiles] = useState<FileDto[]>([]);
  const [filesToUpload, setFilesToUpload] = useState<File[]>([]);
  const [fileUploadMessage, setFileUploadMessage] = useState('');
  const [shareEmail, setShareEmail] = useState('');
  const [selectedFileToShare, setSelectedFileToShare] = useState<FileDto | null>(null);
  const [shareMessage, setShareMessage] = useState('');
  const [sharedFiles, setSharedFiles] = useState<SharedFileDto[]>([]);
  const [openShareDialog, setOpenShareDialog] = useState(false);

  const API_GATEWAY_URL = 'http://localhost:5000';

  useEffect(() => {
    if (token) {
      fetchMyFiles();
      fetchSharedFiles();
    }
  }, [token]);

  const fetchMyFiles = async () => {
    try {
      const response = await axios.get(`${API_GATEWAY_URL}/file/api/files`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setMyFiles(response.data.data.items);
    } catch (error: any) {
      console.error('Failed to fetch my files:', error);
      setFileUploadMessage(`Failed to load files: ${error.response?.data?.message || error.message}`);
    }
  };

  const fetchSharedFiles = async () => {
    try {
      const response = await axios.get(`${API_GATEWAY_URL}/file/api/fileshares`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setSharedFiles(response.data.data);
    } catch (error: any) {
      console.error('Failed to fetch shared files:', error);
      setShareMessage(`Failed to load shared files: ${error.response?.data?.message || error.message}`);
    }
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files) {
      setFilesToUpload(Array.from(event.target.files));
    }
  };

  const handleFileUpload = async () => {
    if (filesToUpload.length === 0 || !token) {
      setFileUploadMessage('Please select at least one file and log in first.');
      return;
    }

    const formData = new FormData();
    filesToUpload.forEach(file => {
      formData.append('files', file);
    });

    try {
      const response = await axios.post(`${API_GATEWAY_URL}/file/api/files/upload`, formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
          Authorization: `Bearer ${token}`,
        },
      });
      setFileUploadMessage(`File upload successful: ${response.data.data.files.map((f: any) => f.fileName).join(', ')}`);
      setFilesToUpload([]); // Clear selected files after upload
      fetchMyFiles(); // Refresh file list
    } catch (error: any) {
      setFileUploadMessage(`File upload failed: ${error.response?.data?.message || error.message}`);
    }
  };

  const handleDeleteFile = async (fileId: number) => {
    try {
      await axios.delete(`${API_GATEWAY_URL}/file/api/files/${fileId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setFileUploadMessage('File deleted successfully!');
      fetchMyFiles(); // Refresh file list
    } catch (error: any) {
      console.error('Failed to delete file:', error);
      setFileUploadMessage(`Failed to delete file: ${error.response?.data?.message || error.message}`);
    }
  };

  const handleOpenShareDialog = (file: FileDto) => {
    setSelectedFileToShare(file);
    setOpenShareDialog(true);
  };

  const handleCloseShareDialog = () => {
    setOpenShareDialog(false);
    setShareEmail('');
    setSelectedFileToShare(null);
  };

  const handleShareFile = async () => {
    if (!selectedFileToShare || !shareEmail || !token) {
      setShareMessage('Please select a file and enter an email.');
      return;
    }

    try {
      const response = await axios.post(`${API_GATEWAY_URL}/file/api/fileshares/create-link`, null, {
        params: {
          fileId: selectedFileToShare.id,
          email: shareEmail,
        },
        headers: { Authorization: `Bearer ${token}` },
      });
      setShareMessage(`Share link created: ${response.data.data}`);
      handleCloseShareDialog();
      fetchSharedFiles(); // Refresh shared files list
    } catch (error: any) {
      setShareMessage(`Failed to create share link: ${error.response?.data?.message || error.message}`);
    }
  };

  const handleAcceptShare = async (shareToken: string) => {
    try {
      await axios.post(`${API_GATEWAY_URL}/file/api/fileshares/accept`, `"${shareToken}"`, {
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
      });
      setShareMessage('Share accepted successfully!');
      fetchSharedFiles(); // Refresh shared files list
      fetchMyFiles(); // Shared file might appear in my files if accepted
    } catch (error: any) {
      setShareMessage(`Failed to accept share: ${error.response?.data?.message || error.message}`);
    }
  };

  const handleRejectShare = async (shareToken: string) => {
    try {
      await axios.post(`${API_GATEWAY_URL}/file/api/fileshares/reject`, `"${shareToken}"`, {
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
      });
      setShareMessage('Share rejected successfully!');
      fetchSharedFiles(); // Refresh shared files list
    } catch (error: any) {
      setShareMessage(`Failed to reject share: ${error.response?.data?.message || error.message}`);
    }
  };

  return (
    <Box sx={{ display: 'flex' }}>
      <CssBaseline />
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <Toolbar>
          <Typography variant="h6" noWrap component="div" sx={{ flexGrow: 1 }}>
            Cloud File Storage Dashboard
          </Typography>
          <Typography variant="subtitle1" sx={{ mr: 2 }}>
            Welcome, {user?.username}!
          </Typography>
          <Button color="inherit" onClick={handleLogout}>
            Logout
          </Button>
        </Toolbar>
      </AppBar>
      <Drawer
        variant="permanent"
        sx={{
          width: drawerWidth,
          flexShrink: 0,
          [`& .MuiDrawer-paper`]: { width: drawerWidth, boxSizing: 'border-box' },
        }}
      >
        <Toolbar />
        <Box sx={{ overflow: 'auto' }}>
          <List>
            <ListItem button onClick={() => setSelectedMenu('my-files')}>
              <ListItemText primary="My Files" />
            </ListItem>
            <ListItem button onClick={() => setSelectedMenu('upload-files')}>
              <ListItemText primary="Upload Files" />
            </ListItem>
            <ListItem button onClick={() => setSelectedMenu('shared-requests')}>
              <ListItemText primary="Shared Requests" />
            </ListItem>
          </List>
          <Divider />
        </Box>
      </Drawer>
      <Box component="main" sx={{ flexGrow: 1, p: 3, width: `calc(100% - ${drawerWidth}px)` }}>
        <Toolbar />
        <Paper elevation={3} sx={{ p: 4 }}>
          {selectedMenu === 'my-files' && (
            <Box>
              <Typography variant="h5" gutterBottom>
                My Files
              </Typography>
              {myFiles.length === 0 ? (
                <Typography>No files found. Upload some!</Typography>
              ) : (
                <List>
                  {myFiles.map((file) => (
                    <ListItem key={file.id} secondaryAction={
                      <Stack direction="row" spacing={1}>
                        <Button variant="outlined" size="small" onClick={() => handleOpenShareDialog(file)}>
                          Share
                        </Button>
                        <Button variant="outlined" color="error" size="small" onClick={() => handleDeleteFile(file.id)}>
                          Delete
                        </Button>
                      </Stack>
                    }>
                      <ListItemText 
                        primary={file.fileName} 
                        secondary={`Size: ${(file.fileSize / 1024).toFixed(2)} KB | Uploaded: ${new Date(file.createdAt).toLocaleDateString()}`}
                      />
                    </ListItem>
                  ))}
                </List>
              )}
            </Box>
          )}

          {selectedMenu === 'upload-files' && (
            <Box>
              <Typography variant="h5" gutterBottom>
                Upload Files
              </Typography>
              <Stack spacing={2}>
                <input
                  type="file"
                  multiple
                  onChange={handleFileChange}
                  style={{ display: 'none' }}
                  id="file-upload-button"
                />
                <label htmlFor="file-upload-button">
                  <Button variant="outlined" component="span" fullWidth>
                    {filesToUpload.length > 0 ? `${filesToUpload.length} file(s) selected` : 'Select Files'}
                  </Button>
                </label>
                {filesToUpload.length > 0 && (
                  <List dense>
                    {filesToUpload.map((file, index) => (
                      <ListItem key={index}>
                        <ListItemText primary={file.name} secondary={`${(file.size / 1024).toFixed(2)} KB`} />
                      </ListItem>
                    ))}
                  </List>
                )}
                <Button variant="contained" color="success" onClick={handleFileUpload} disabled={filesToUpload.length === 0 || !token}>
                  Upload Files
                </Button>
              </Stack>
            </Box>
          )}

          {selectedMenu === 'shared-requests' && (
            <Box>
              <Typography variant="h5" gutterBottom>
                Shared Requests
              </Typography>
              {sharedFiles.length === 0 ? (
                <Typography>No shared requests found.</Typography>
              ) : (
                <List>
                  {sharedFiles.map((share) => (
                    <ListItem key={share.id} secondaryAction={
                      <Stack direction="row" spacing={1}>
                        {!share.isAccepted && (
                          <>
                            <Button variant="outlined" size="small" onClick={() => handleAcceptShare(share.shareToken)}>
                              Accept
                            </Button>
                            <Button variant="outlined" color="error" size="small" onClick={() => handleRejectShare(share.shareToken)}>
                              Reject
                            </Button>
                          </>
                        )}
                        {share.isAccepted && (
                          <Typography variant="caption" color="textSecondary">Accepted</Typography>
                        )}
                      </Stack>
                    }>
                      <ListItemText 
                        primary={`File: ${share.fileName} (Shared by: ${share.sharedBy})`} 
                        secondary={`Shared: ${new Date(share.sharedAt).toLocaleDateString()} | Status: ${share.isAccepted ? 'Accepted' : 'Pending'}`}
                      />
                    </ListItem>
                  ))}
                </List>
              )}
            </Box>
          )}

          <Stack spacing={1} sx={{ mt: 4 }}>
            {fileUploadMessage && <Alert severity={fileUploadMessage.includes('failed') ? 'error' : 'success'}>{fileUploadMessage}</Alert>}
            {shareMessage && <Alert severity={shareMessage.includes('failed') ? 'error' : 'success'}>{shareMessage}</Alert>}
          </Stack>
        </Paper>
      </Box>

      {/* Share Dialog */}
      <Dialog open={openShareDialog} onClose={handleCloseShareDialog}>
        <DialogTitle>Share File: {selectedFileToShare?.fileName}</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Enter the email address of the person you want to share this file with.
          </DialogContentText>
          <TextField
            autoFocus
            margin="dense"
            id="share-email"
            label="Recipient Email"
            type="email"
            fullWidth
            variant="standard"
            value={shareEmail}
            onChange={(e) => setShareEmail(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseShareDialog}>Cancel</Button>
          <Button onClick={handleShareFile}>Share</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export default DashboardPage;