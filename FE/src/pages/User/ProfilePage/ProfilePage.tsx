import './ProfilePage.css'
import Header from '../../../components/Header/Header.tsx'
import Footer from '../../../components/Footer/Footer.tsx'
import Profile from '../../../components/User/Profile/Profile.tsx'

export default function ProfilePage() {
	return (
		<div id="ProfilePage">
			<Header/>
			<Profile />
			<Footer/>
		</div>
	)
}