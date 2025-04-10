import { useState, useEffect } from 'react'
import Header from '../../../components/Header/Header'
import Designer from '../../../components/Cards/Designer/Designer'
import './SearchDesigner.css'
import { ApiGateway } from '../../../services/api/ApiService'

interface DesignerCard {
	userId: string;
	shots: { id: string; image: string }[];
	specialties: string[];
	price?: number | null;
	place?: string | null;
	username: string;
	userImage: string;
}

const SearchDesigner = () => {

	// const categories = [
	// 	'All',
	// 	'Animation',
	// 	'Branding',
	// 	'Illustration',
	// 	'Mobile',
	// 	'Print',
	// 	'Product Design',
	// 	'Typography',
	// 	'Web Design',
	// ];

	const [designers, setDesigners] = useState<DesignerCard[]>([]);

	const fetchDesigners = async () => {
		const data = await ApiGateway.ListDesigner<DesignerCard[]>(4, 1, 4);
		setDesigners(data);
		console.log(data);
	}

	useEffect(() => {
		fetchDesigners()
	}, []);

	return (
		<div id='search_designer'>
			<Header />
			<div className='page-content'>
				<div className="heading section">
					<p>Find Your Perfect Designer</p>
					<p>Discover talented designers to bring your creative visions to life. Search, connect, and collaborate today.</p>
				</div>
				<div className="search-filter section">
					{/* <div className="toggle-bar">
						{categories.map((category) => (
							<button
							key={category}
							className={`toggle-button ${active === category ? 'active' : ''}`}
							onClick={() => handleToggle(category)}
							>
							{category}
							</button>
						))}
					</div> */}
					<div className="addition">
						<div className='location'>
							<label>
								<input type="text" placeholder='Enter Location' />
								<p>
									<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M9.5 16q-2.725 0-4.612-1.888T3 9.5t1.888-4.612T9.5 3t4.613 1.888T16 9.5q0 1.1-.35 2.075T14.7 13.3l5.6 5.6q.275.275.275.7t-.275.7t-.7.275t-.7-.275l-5.6-5.6q-.75.6-1.725.95T9.5 16m0-2q1.875 0 3.188-1.312T14 9.5t-1.312-3.187T9.5 5T6.313 6.313T5 9.5t1.313 3.188T9.5 14" /></svg>
								</p>
							</label>
						</div>
						<div>
							<label className='budget'>
								<input type="number" placeholder='Enter Budget' />
								<p>VND</p>
							</label>
						</div>
					</div>
				</div>
				<div className="designers section">
					{designers.map((designer, index) =>
						<Designer
							key={index}
							id={designer.userId}
							avatarURL={designer.userImage}
							name={designer.username}
							place={designer.place}
							isSaved={false}
							specialty={designer.specialties}
							products={designer.shots}
						/>
					)}
				</div>
			</div>
		</div>
	)
}

export default SearchDesigner
