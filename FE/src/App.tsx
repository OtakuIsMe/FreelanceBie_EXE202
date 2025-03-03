import './App.css'
import { BrowserRouter, Routes, Route, useLocation, useNavigate } from 'react-router-dom'
import Home from './pages/User/Home/Home'
import Inspiration from './pages/User/Inspriration/Inspiration'
import SearchDesigner from './pages/User/SearchDesigner/SearchDesigner'
import FindJob from './pages/User/FindJob/FindJob'
import ProfilePage, { DesProfilePage } from './pages/User/ProfilePage/ProfilePage'
import AuthenProvider from './hooks/AuthenContext'
import ShotEdit from './pages/User/Shot/Edit/ShotEdit'
import ShotDetail from './pages/User/Shot/Detail/ShotDetail'
import JobDetail from './pages/User/JobDetail/JobDetail'
import Pricing from './pages/User/Pricing/Pricing'
import CheckOut from './pages/User/Checkout/CheckOut'
import Modal from './hooks/Modal'
import { useEffect } from 'react'
import PostJob from './pages/User/PostJob/PostJob'

function App() {
	return (
		<BrowserRouter>
			<MainRoutes />
		</BrowserRouter>
	)
}

export default App

function MainRoutes() {
	const location = useLocation();
	const navigate = useNavigate();
	const background = location.state?.background;
	return (
		<AuthenProvider>
			<Routes location={background || location}>
				<Route path='/' element={<Home />} />
				<Route path='/inspiration' element={<Inspiration />} />
				<Route path='/search-designer' element={<SearchDesigner />} />
				<Route path='/find-job' element={<FindJob />} />
				<Route path='/Profile' element={<ProfilePage />} />
				<Route path='/des-profile' element={<DesProfilePage />} />
				<Route path='/job-detail' element={<JobDetail />} />
				<Route path="/post-job" element={<PostJob />} />
				<Route path="/shot">
					<Route path="edit" element={<ShotEdit />} />
				</Route>
				<Route path='/pro'>
					<Route index element={<Pricing />} />
					<Route path="check-out" element={<CheckOut />} />
				</Route>
			</Routes>
			{background ? (
				<Modal onClose={() => navigate(-1)}>
					<Routes>
						<Route path="/shot/:id" element={<ShotDetail />} />
					</Routes>
				</Modal>
			) : (
				<Routes>
					<Route path="/shot/:id" element={<ShotDetail />} />
				</Routes>
			)}
		</AuthenProvider>
	)
}
