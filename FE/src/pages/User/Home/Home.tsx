import './Home.css'
import Header from '../../../components/Header/Header.tsx'
import Explore from '../../../components/Cards/Explore/Explore.tsx'
import Footer from '../../../components/Footer/Footer.tsx'
import { Link, useLocation } from "react-router-dom";
import { useEffect, useState } from 'react'
import { ApiGateway } from '../../../services/api/ApiService.tsx'

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

	useEffect(() => {
		fetchShotRandom();
	}, [])
	const fetchShotRandom = async () => {
		const data = await ApiGateway.ShotRandom<ShotView[]>(8)
		setShots(data)
	}

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
