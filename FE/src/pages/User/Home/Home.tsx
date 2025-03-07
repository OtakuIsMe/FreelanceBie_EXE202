import './Home.css'
import Animation from '../../../assets/Animation.jpg'
import Branding from '../../../assets/Branding.png'
import Illustration from '../../../assets/Illustration.jpg'
import Mobile from '../../../assets/Mobile.png'
import Print from '../../../assets/Print.jpg'
import ProductDesign from '../../../assets/Product_Design.jpg'
import Typography from '../../../assets/Typography.jpg'
import WebDesign from '../../../assets/Web_Design.jpg'
import Header from '../../../components/Header/Header.tsx'
import Specialities from '../../../components/Cards/Specialities/Specialities.tsx'
import Explore from '../../../components/Cards/Explore/Explore.tsx'
import Footer from '../../../components/Footer/Footer.tsx'
import { Link, useLocation } from "react-router-dom";
import { useEffect, useState } from 'react'
import { ApiGateway } from '../../../services/api/ApiService.tsx'
import CheckOut from '../../../components/CheckOut/CheckOut.tsx'

interface ShotView {
	id: string;
	image: string;
	countView: number;
	countLike: number;
	user: {
		username: string;
		image: string;
	};
	title: string;
}

export default function Home() {
	const [shots, setShots] = useState<ShotView[] | null>(null)
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

	useEffect(() => {
		fetchShotRandom();
	}, [])
	const fetchShotRandom = async () => {
		const data = await ApiGateway.ShotRandom<ShotView[]>(8)
		setShots(data)
	}

	return (
		<div id="home">
			<CheckOut />
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
						{shots?.map((shot, index) =>
							<Link
								key={index}
								to={`/shot/${shot.id}`}
								state={{ background: location }}
							>
								<Explore username={shot.user.username} liked={shot.countLike} viewed={shot.countView} img={shot.image} />
							</Link>
						)}
					</div>
				</div>
			</div>
			<Footer />
		</div>
	)
}
