import './App.css'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Home from './pages/User/Home/Home'
import Login from './pages/User/Login/Login'

function App() {
	return (
		<BrowserRouter>
			<Routes>
				<Route path='/' element={<Home />} />
			</Routes>
			<Routes>
				<Route path="/login" element={<Login />} />
			</Routes>

		</BrowserRouter>
	)
}

export default App
