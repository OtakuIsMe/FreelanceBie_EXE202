import React, { useState, useEffect } from 'react'
import Header from '../../../components/Header/Header'
import Explore from '../../../components/Cards/Explore/Explore'
import D1 from '../../../assets/D1.jpg'
import D2 from '../../../assets/D2.jpg'
import D3 from '../../../assets/D3.jpg'
import D4 from '../../../assets/D4.jpg'
import D5 from '../../../assets/D5.jpg'
import Tag from '../../../components/Cards/Tag/Tag'
import './Inspiration.css'

const Inspiration : React.FC = () => {

    const exp = [
        {username: 'Otaku', liked: 61, viewed: 126, img: D1, topic: ['Web design']},
        {username: 'Otaku', liked: 61, viewed: 126, img: D2, topic: ['Web design']},
        {username: 'Otaku', liked: 61, viewed: 126, img: D3, topic: ['Web design']},
        {username: 'Otaku', liked: 61, viewed: 126, img: D4, topic: ['Web design']},
        {username: 'Otaku', liked: 61, viewed: 126, img: D5, topic: ['Web design']},
        {username: 'Otaku', liked: 61, viewed: 126, img: D1, topic: ['Web design']},
        {username: 'Otaku', liked: 61, viewed: 126, img: D2, topic: ['Web design']},
        {username: 'Otaku', liked: 61, viewed: 126, img: D3, topic: ['Web design']},
        {username: 'Otaku', liked: 61, viewed: 126, img: D4, topic: ['Web design']},
        {username: 'Otaku', liked: 61, viewed: 126, img: D5, topic: ['Web design']},
    ]

    const tags = [
        {tag: 'Landing Page'},
        {tag: 'Web design'},
        {tag: 'User interface'},
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

    function searchKeys(key: string){
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
                            <Tag tag={tag.tag}/>
                        )}
                    </div>
                </form>
                <div className="inspis">
                    <div className="inspis-container">
                        {exp.map((exp, index) =>
                            <Explore username={exp.username} liked={exp.liked} viewed={exp.viewed} img={exp.img} topic={exp.topic} />
                        )}
                    </div>
                </div>
            </div>
        </div>
    )
}

export default Inspiration