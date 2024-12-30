import './App.css'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Home from './pages/User/Home/Home'
import ProfilePage from './pages/User/ProfilePage/ProfilePage'

function App() {
	return (
		<BrowserRouter>
			<Routes>
				<Route path='/' element={<Home />} />
				<Route path='/Profile' element={<ProfilePage />} />
			</Routes>
		</BrowserRouter>
	)
}

export default App
