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

interface Special{
    type: string;
    img: string;
}

export default function Home() {

	const spec = [
		{type: 'Animation', image: Animation},
		{type: 'Branding', image: Branding},
		{type: 'Illustration', image: Illustration},
		{type: 'Mobile', image: Mobile},
		{type: 'Print', image: Print},
		{type: 'ProductDesign', image: ProductDesign},
		{type: 'Typography', image: Typography},
		{type: 'WebDesign', image: WebDesign},
	]
	
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
					<p className='title'>Specialties</p>
					<div className="spec-container">
						{spec.map((spec, index) =>
							<Specialities type={spec.type} img={spec.image}/>
						)}
					</div>
				</div>
				<div className="explore section">

				</div>
			</div>
		</div>
	)
}
