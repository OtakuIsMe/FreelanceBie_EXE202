import React, { useState, useEffect } from 'react'
import Header from '../../../components/Header/Header'
import Explore from '../../../components/Cards/Explore/Explore'
import Tag from '../../../components/Cards/Tag/Tag'
import './Inspiration.css'
import { ApiGateway } from '../../../services/api/ApiService'
import { Link, useLocation } from "react-router-dom";

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

const Inspiration: React.FC = () => {
	const location = useLocation();

	const [shots, setShots] = useState<ShotView[]>([])

	const tags = [
		{ tag: 'Landing Page' },
		{ tag: 'Web design' },
		{ tag: 'User interface' },
	]

	const [searchString, setSearchString] = useState<string>('')

	const [suggestTags] = useState<string[]>([
		'Backpack',
		'Bag',
		'Laptop Bag',
		'Handbag',
		'Tote Bag',
		'Duffel Bag',
	]);
	const [suggestedText, setSuggestedText] = useState<string>('');

	useEffect(() => {
		fetchShots()
	}, [])

	const fetchShots = async () => {
		const data = await ApiGateway.ListShot<ShotView[]>(1, 10)
		setShots(data)
	}

	const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => {
		const value = event.target.value;
		setSearchString(value);

		// Split the input string by commas
		const parts = value.split(',').map((part) => part.trim());
		const lastInput = parts[parts.length - 1];

		// Find the first matching suggestTags for auto-complete
		if (lastInput) {
			const suggestion = suggestTags.find((tag) =>
				tag.toLowerCase().startsWith(lastInput.toLowerCase())
			);
			setSuggestedText(suggestion || '');
		} else {
			setSuggestedText('');
		}
	};

	const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
		if (event.key === 'Tab' && suggestedText) {
			event.preventDefault();

			// Replace the last input with the suggestion
			const parts = searchString.split(',').map((part) => part.trim());
			parts[parts.length - 1] = suggestedText;

			setSearchString(parts.join(', ') + ', '); // Add a comma after the completed tag
			setSuggestedText('');
		}
	};

	function searchKeys(key: string) {
		var arry = key.split(',');
		return console.log(arry)
	}

	return (
		<div id='inspiration'>
			<Header />
			<div className="page-content">
				<form className="search-filter section">
					<div className="title">
						<p>Explore work from the most talented Vietnamese designers</p>
						<p>Ready to bring your vision to life with local expertise and creative excellence</p>
					</div>
					<div className="search">
						{/* <input type="text" placeholder='Looking for inspiration?' value={searchString}/> */}
						<input
							type="text"
							value={searchString}
							onChange={handleSearchChange}
							onKeyDown={handleKeyDown}
							placeholder="Looking for inspiration?"
						/>
						{suggestedText && (
							<div
								style={{
									position: 'absolute',
									top: '36px',
									left: '8px',
									color: '#000',
									backgroundColor: '#fff',
									padding: '4px',
									fontSize: '12px',
								}}
							>
								Auto-complete: {suggestedText}
							</div>
						)}
						<button type='submit' name='search' onClick={() => searchKeys}>Search</button>
					</div>
					<div className="trending">
						<p>Trending:</p>
						{tags.map((tag, index) =>
							<Tag tag={tag.tag} key={index} />
						)}
					</div>
				</form>
				<div className="inspis">
					<div className="inspis-container">
						{shots.map((shot, index) =>
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
		</div>
	)
}

export default Inspiration
