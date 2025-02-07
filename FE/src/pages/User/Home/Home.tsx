import './Home.css'
import Header from '../../../components/Header/Header.tsx'
import Footer from '../../../components/Footer/Footer.tsx'

export default function Home() {
	return (
		<div id="home">
			<Header/>
			<div className="page-content">
				<div className="banner section">
					<div className="content">
						<div className="title">
							Find <span>Designers</span> Who Bring <br/> Your Vision To Life
						</div>
						<p>
							Get inspired with other designers
						</p>
						<button className='start-btn'>Get Started</button>
					</div>
				</div>
				<div className="specialties section">

				</div>
				<div className="explore section">

				</div>
			</div>
			<Footer/>
		</div>
	)
}
