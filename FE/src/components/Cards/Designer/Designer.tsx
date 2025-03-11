import React, { useState } from 'react'
import Tag from '../Tag/Tag'
import './Designer.css'
import { Link, useLocation } from 'react-router-dom';

interface Designer {
	avatarURL: string,
	name: string,
	place?: string | null,
	isSaved: boolean,
	specialty: string[],
	products: { id: string; image: string }[]
}

const Designer: React.FC<Designer> = ({ avatarURL, name, place, isSaved, specialty, products }) => {

	const [hideSkills, setHideSkills] = useState(true)

	const location = useLocation();

	function hideShillToggle() {
		setHideSkills(!hideSkills);
	}

	return (
		<div id='designer_card'>
			<div className="des-container">
				<div className="card-section user-info">
					<div className='first-part'>
						<img src={avatarURL} alt={name} height={24} width={24} />
						<div>
							<p>{name}</p>
							<p>{place}</p>
						</div>
					</div>
					<div className='second-part'>
						<div className="saved-icon">
							{!isSaved && <svg width="15" height="80" viewBox="0 0 125 200" fill="none" xmlns="http://www.w3.org/2000/svg">
								<path d="M0 10C0 4.47716 4.47715 0 10 0H115C120.523 0 125 4.47715 125 10V200L62.5 150L0 200V10Z" fill="#FD73DA" />
							</svg>
							}
						</div>
						<div className='contact-btn'>
							Contact
						</div>
					</div>
				</div>
				<div className="card-section user-products">
					<div className="product-container">
						{products.map((product, index) =>
							<Link
								key={index}
								to={`/shot/${product.id}`}
								state={{ background: location }}
							>
								<div className="img-container">
									<img key={index} src={product.image} />
								</div>
							</Link>
						)}
					</div>
				</div>
				<div className="card-section user-skills">
					<div className="skill-container">
						{
							(specialty.length > 3 && hideSkills) ? specialty.slice(0, 3).map((specialty, index) =>
								<Tag key={index} tag={specialty} />
							) :
								specialty.map((specialty, index) =>
									<Tag key={index} tag={specialty} />
								)
						}
						{(specialty.length - 3) == 0 ?
							<></>
							:
							hideSkills ?
								<span onClick={() => hideShillToggle()}>+{specialty.length - 3} {(specialty.length - 3) > 2 ? "skills" : "skill"}</span>
								:
								<span onClick={() => hideShillToggle()} style={{ paddingLeft: '5px' }}>Hide</span>
						}
					</div>
				</div>
			</div>
		</div>
	)
}

export default Designer
