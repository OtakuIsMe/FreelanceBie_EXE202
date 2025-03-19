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
import Modal from './hooks/Modal'
import PostJob from './pages/User/PostJob/PostJob'
import PostManage from './pages/User/Post/Manage/PostManage'
import PostEmployee from './pages/User/Post/Employee/PostEmployee'
import { ReactNotifications } from 'react-notifications-component'
import 'react-notifications-component/dist/theme.css'

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
			<ReactNotifications />
			<Routes location={background || location}>
				<Route path='/' element={<Home />} />
				<Route path='/inspiration' element={<Inspiration />} />
				<Route path='/search-designer' element={<SearchDesigner />} />
				<Route path='/find-job' element={<FindJob />} />
				<Route path='/Profile' element={<ProfilePage />} />
				<Route path='/des-profile' element={<DesProfilePage />} />
				<Route path='/job-detail/:id' element={<JobDetail />} />
				<Route path="/post-job" element={<PostJob />} />
				<Route path="/shot">
					<Route path="edit" element={<ShotEdit />} />
					<Route path='detail' element={<ShotDetail />} />
				</Route>
				<Route path='/pro'>
					<Route index element={<Pricing />} />
				</Route>
				<Route path='/post'>
					<Route path='manage' element={<PostManage />} />
					<Route path='employee' element={<PostEmployee />} />
				</Route>
			</Routes>
			{background && (
				<Modal onClose={() => navigate(-1)}>
					<Routes>
						<Route path="/shot/:id" element={<ShotDetail />} />
					</Routes>
				</Modal>
			)}
		</AuthenProvider>
	)
}
