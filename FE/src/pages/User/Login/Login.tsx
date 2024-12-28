import React from 'react';
import { getAuth, signInWithPopup, GoogleAuthProvider } from 'firebase/auth';
import { firebaseApp } from '../Login/firebase/fibaseConfig';
import './Login.css'; 

const Login: React.FC = () => {
  const auth = getAuth(firebaseApp);

  const handleGoogleLogin = async () => {
    const provider = new GoogleAuthProvider();
    try {
      const result = await signInWithPopup(auth, provider);
      const user = result.user;
      console.log('User Info:', user);
      alert(`Welcome ${user.displayName}`);
    } catch (error: any) {
      if (error.code === 'auth/popup-closed-by-user') {
        alert('Login canceled by the user.');
      } else {
        console.error('Login failed:', error);
        alert('Login failed. Please try again.');
      }
    }
  };

  return (
    <div className="container">
      <h1>FreelanceBie Login Page</h1>
      <button className="login-button" onClick={handleGoogleLogin}>
        Sign in with Google
      </button>
    </div>
  );
};

export default Login;
