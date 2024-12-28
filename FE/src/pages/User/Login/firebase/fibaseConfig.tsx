import { initializeApp } from "firebase/app";
import { getAnalytics } from "firebase/analytics";
// TODO: Add SDKs for Firebase products that you want to use
// https://firebase.google.com/docs/web/setup#available-libraries

// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = {
  apiKey: "AIzaSyAhgAASANViJJJybegGyG0kcwuSMR38GNQ",
  authDomain: "exe202-a5e56.firebaseapp.com",
  projectId: "exe202-a5e56",
  storageBucket: "exe202-a5e56.firebasestorage.app",
  messagingSenderId: "83807708161",
  appId: "1:83807708161:web:f9cbf10294177f9f5bcc00",
  measurementId: "G-LSYRJEMZ3K"
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);
const analytics = getAnalytics(app);

export { app as firebaseApp };

