import './App.css'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Home from './pages/User/Home/Home'
import Inspiration from './pages/User/Inspriration/Inspiration'
import SearchDesigner from './pages/User/SearchDesigner/SearchDesigner'
import FindJob from './pages/User/FindJob/FindJob'
import ProfilePage from './pages/User/ProfilePage/ProfilePage'
import Login from './pages/User/Login/Login'

function App() {
	return (
		<BrowserRouter>
			<Routes>
				<Route path='/' element={<Home />} />
				<Route path='/inspiration' element={<Inspiration />} />
				<Route path='/search-designer' element={<SearchDesigner />} />
				<Route path='/find-job' element={<FindJob />} />
				<Route path='/Profile' element={<ProfilePage />} />
				<Route path="/login" element={<Login />} />
			</Routes>
		</BrowserRouter>
	)
}

export default App
