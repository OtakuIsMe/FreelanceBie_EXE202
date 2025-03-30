import React from 'react'
import './Designer.css'
import { Link, useLocation } from 'react-router-dom';
import { ApiGateway } from '../../../services/api/ApiService';

interface Designer {
	id: string,
	avatarURL: string,
	name: string,
	place?: string | null,
	isSaved: boolean,
	specialty: string[],
	products: { id: string; image: string }[]
}

interface GetInTouchRp {
	communicationId: string
}

const Designer: React.FC<Designer> = ({ id, avatarURL, name, place, products }) => {

	const location = useLocation();


	const handleGetInTouchClick = async () => {
		console.log(id)
		if (id) {
			console.log(id)
			const data = await ApiGateway.GetInTouch<GetInTouchRp>(id)
			if (data) {
				window.location.href = `/messages/${data.communicationId}`
			}
		}
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
						<div className='contact-btn' onClick={() => handleGetInTouchClick()}>
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
				{/* <div className="card-section user-skills">
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
				</div> */}
			</div>
		</div>
	)
}

export default Designer
