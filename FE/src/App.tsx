import './App.css'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Home from './pages/User/Home/Home'
import Inspiration from './pages/User/Inspriration/Inspiration'
import SearchDesigner from './pages/User/SearchDesigner/SearchDesigner'
import FindJob from './pages/User/FindJob/FindJob'
import ProfilePage from './pages/User/ProfilePage/ProfilePage'
import AuthenProvider from './hooks/AuthenContext'
import ShotEdit from './pages/User/Shot/Edit/ShotEdit'
import ShotDetail from './pages/User/Shot/Detail/ShotDetail'

function App() {
	return (
		<BrowserRouter>
			<AuthenProvider>
				<Routes>
					<Route path='/' element={<Home />} />
					<Route path='/inspiration' element={<Inspiration />} />
					<Route path='/search-designer' element={<SearchDesigner />} />
					<Route path='/find-job' element={<FindJob />} />
					<Route path='/profile' element={<ProfilePage />} />
					<Route path="/shot">
						<Route index element={<ShotDetail />} />
						<Route path="edit" element={<ShotEdit />} />
					</Route>
				</Routes>
			</AuthenProvider>
		</BrowserRouter>
	)
}

export default App
