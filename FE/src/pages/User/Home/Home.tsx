import './Home.css'
import Animation from '../../../assets/Animation.jpg'
import Branding from '../../../assets/Branding.png'
import Illustration from '../../../assets/Illustration.jpg'
import Mobile from '../../../assets/Mobile.png'
import Print from '../../../assets/Print.jpg'
import ProductDesign from '../../../assets/Product_Design.jpg'
import Typography from '../../../assets/Typography.jpg'
import WebDesign from '../../../assets/Web_Design.jpg'
import D1 from '../../../assets/D1.jpg'
import D2 from '../../../assets/D2.jpg'
import D3 from '../../../assets/D3.jpg'
import D4 from '../../../assets/D4.jpg'
import D5 from '../../../assets/D5.jpg'
import Header from '../../../components/Header/Header.tsx'
import Specialities from '../../../components/Cards/Specialities/Specialities.tsx'
import Explore from '../../../components/Cards/Explore/Explore.tsx'
import Footer from '../../../components/Footer/Footer.tsx'
import { Link, useLocation } from "react-router-dom";

export default function Home() {
	const location = useLocation();

	const spec = [
		{ type: 'Animation', image: Animation },
		{ type: 'Branding', image: Branding },
		{ type: 'Illustration', image: Illustration },
		{ type: 'Mobile', image: Mobile },
		{ type: 'Print', image: Print },
		{ type: 'ProductDesign', image: ProductDesign },
		{ type: 'Typography', image: Typography },
		{ type: 'WebDesign', image: WebDesign },
	]

	const exp = [
		{ username: 'Otaku', liked: 61, viewed: 126, img: D1, topic: ['Web design'] },
		{ username: 'Otaku', liked: 61, viewed: 126, img: D2, topic: ['Web design'] },
		{ username: 'Otaku', liked: 61, viewed: 126, img: D3, topic: ['Web design'] },
		{ username: 'Otaku', liked: 61, viewed: 126, img: D4, topic: ['Web design'] },
		{ username: 'Otaku', liked: 61, viewed: 126, img: D5, topic: ['Web design'] },
		{ username: 'Otaku', liked: 61, viewed: 126, img: D1, topic: ['Web design'] },
		{ username: 'Otaku', liked: 61, viewed: 126, img: D2, topic: ['Web design'] },
		{ username: 'Otaku', liked: 61, viewed: 126, img: D3, topic: ['Web design'] },
		{ username: 'Otaku', liked: 61, viewed: 126, img: D4, topic: ['Web design'] },
		{ username: 'Otaku', liked: 61, viewed: 126, img: D5, topic: ['Web design'] },
	]

	return (
		<div id="home">
			<Header />
			<div className="page-content">
				<div className="banner section">
					<div className="content">
						<div className="title">
							Find <span>Designers</span> Who Bring <br /> Your Vision To Life
						</div>
						<p>
							Get inspired with other designers
						</p>
						<button className='start-btn'>Get Started</button>
					</div>
				</div>
				<div className="specialties section">
					<p className='title'>Specialties</p>
					<div className="spec-container">
						{spec.map((spec, index) =>
							<Specialities type={spec.type} img={spec.image} />
						)}
					</div>
				</div>
				<div className="explore section">
					<p className='title'>Explore inspiring designs</p>
					<div className="exp-container">
						{exp.map((exp, index) =>
							<Link
								key={index}
								to={`/shot/${index}`}
								state={{ background: location }}
							>
								<Explore username={exp.username} liked={exp.liked} viewed={exp.viewed} img={exp.img} topic={exp.topic} />
							</Link>
						)}
					</div>
				</div>
			</div>
			<Footer />
		</div>
	)
}
