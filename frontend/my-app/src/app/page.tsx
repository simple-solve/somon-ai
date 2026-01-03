'use client';
import React, {useState} from 'react';
import {ChevronRight, Search} from 'lucide-react';
import photo from '../app/optimize.webp';
import Image from 'next/image';

interface Category {
  icon: string;
  label: string;
  color: string;
}

interface Listing {
  id: number;
  image: string;
  title: string;
  price: string;
  location: string;
  date: string;
  imageCount: number;
}

const HomePage: React.FC = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedRegion, setSelectedRegion] = useState('Весь Таджикистан');

  const categories: Category[] = [
    {icon: '🏗', label: 'Новостройки', color: 'bg-blue-50'},
    {icon: '🏠', label: 'Недвижимость', color: 'bg-red-50'},
    {icon: '🚚', label: 'Транспорт', color: 'bg-orange-50'},
    {icon: '💼', label: 'Вакансии', color: 'bg-purple-50'},
    {icon: '📱', label: 'Телефоны и связь', color: 'bg-cyan-50'},
    {icon: '👶', label: 'Детский мир', color: 'bg-blue-50'},
    {icon: '👔', label: 'Одежда и личные вещи', color: 'bg-yellow-50'},
    {icon: '💻', label: 'Компьютеры и оргтехника', color: 'bg-indigo-50'},
    {icon: '📺', label: 'Электроника и бытовая техника', color: 'bg-gray-50'},
    {icon: '🏡', label: 'Все для дома', color: 'bg-green-50'},
    {icon: '🔨', label: 'Строительство, сырье и ремонт', color: 'bg-orange-50'},
    {icon: '🏀', label: 'Хобби, музыка и спорт', color: 'bg-orange-50'},
    {icon: '🐕', label: 'Животные и растения', color: 'bg-pink-50'},
    {icon: '📊', label: 'Услуги', color: 'bg-blue-50'},
    {icon: '👨‍💼', label: 'Все для бизнеса', color: 'bg-gray-50'},
    {icon: '🎁', label: 'Отдам даром', color: 'bg-yellow-50'},
  ];

  const listings: Listing[] = [
    {
      id: 1,
      image: 'https://images.unsplash.com/photo-1617814076367-b759c7d7e738?w=400&h=300&fit=crop',
      title: 'Tesla Model 3 2023',
      price: '$45,000',
      location: 'Душанбе',
      date: '01.01.2026',
      imageCount: 8
    },
    {
      id: 2,
      image: 'https://images.unsplash.com/photo-1606664515524-ed2f786a0bd6?w=400&h=300&fit=crop',
      title: 'Toyota Land Cruiser',
      price: '$85,000',
      location: 'Душанбе',
      date: '31.12.2025',
      imageCount: 11
    },
    {
      id: 3,
      image: 'https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb?w=400&h=300&fit=crop',
      title: 'Changan CS75',
      price: '$28,000',
      location: 'Худжанд',
      date: '30.12.2025',
      imageCount: 14
    },
    {
      id: 4,
      image: 'https://images.unsplash.com/photo-1533473359331-0135ef1b58bf?w=400&h=300&fit=crop',
      title: 'Toyota Land Cruiser Prado',
      price: '$55,000',
      location: 'Душанбе',
      date: '29.12.2025',
      imageCount: 7
    },
    {
      id: 5,
      image: 'https://images.unsplash.com/photo-1552519507-da3b142c6e3d?w=400&h=300&fit=crop',
      title: 'BMW X5 2024',
      price: '$72,000',
      location: 'Душанбе',
      date: '28.12.2025',
      imageCount: 6
    },
    {
      id: 6,
      image: 'https://images.unsplash.com/photo-1563720360172-67b8f3dce741?w=400&h=300&fit=crop',
      title: 'Mercedes-Benz E-Class',
      price: '$48,000',
      location: 'Душанбе',
      date: '27.12.2025',
      imageCount: 8
    },
  ];

  return (
    <div className="min-h-screen bg-gray-50">


      {/* Search Bar */}
      <div className="bg-[#0093d3] py-5">
        <div className="max-w-7xl mx-auto px-4">
          <div className="flex gap-4">
            <div
              className="w-137.5 h-9 bg-white rounded-sm shadow-lg flex overflow-hidden ring-2 ring-yellow-400"
            >
              <input
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                placeholder="521 032 объявлений рядом"
                className="flex-1 px-3 text-lg font-medium"
              />
              <select
                value={selectedRegion}
                onChange={(e) => setSelectedRegion(e.target.value)}
                className="px-4 border-l outline-none cursor-pointer text-lg"
              >
                <option>Весь Таджикистан</option>
                <option>Душанбе</option>
                <option>Худжанд</option>
                <option>Куляб</option>
              </select>
            </div>
            <button
              className="
                bg-yellow-400 text-gray-900 px-10 rounded-sm
                font-bold text-lg shadow-lg cursor-pointer
                flex items-center justify-start gap-3 pl-8
                transition-all duration-300 ease-out
                hover:bg-[#ea4300]
                hover:shadow-xl
              "
            >
              <Search
                className="w-6 h-6 transition-transform duration-300 group-hover:translate-x-1"
              />
              Найти
            </button>

          </div>
        </div>
      </div>

      {/* Categories */}
      <div className="max-w-7xl mx-auto px-4 py-8">
        <div
          className='grid grid-cols-4 place-items-start place-content-start gap-y-4 text-start'
        >
          {categories.map((category, index) => (
            <button
              key={index}
              className="group button cursor-pointer"
            >
              <span className="text-3xl">{category.icon}</span>

              <span
                className="
                  text-[#0093d3] font-bold
                  transition-colors duration-200
                  group-hover:text-orange-500
                "
              >
                {category.label}
              </span>
            </button>
          ))}

        </div>

        {/* Banner */}
        <div className="max-w-4xl mx-auto my-5">
          <Image src={photo} alt="Banner" className="rounded-lg" />
        </div>

        {/* Listings Section */}
        <div>
          <h2 className="text-2xl font-bold mb-4">Электромобили</h2>
          <div className="grid grid-cols-6 gap-6">
            {listings.map((listing) => (
              <div key={listing.id} className="group">
                <div className="relative">
                  <img
                    src={listing.image}
                    alt={listing.title}
                    className="w-full h-full rounded-sm object-cover"
                  />

                  <div
                    className="absolute top-2 right-2 bg-gray-500 bg-opacity-60 group-hover:bg-orange-500 px-1 text-white rounded text-sm flex items-center justify-center gap-1"
                  >
                    <span className="text-[12px]">📷</span>
                    <span className="text-[11px]">{listing.imageCount}</span>
                  </div>
                </div>

                <div>
                  <p className="font-semibold text-gray-900">{listing.price}</p>
                  <h3
                    className="
                      text-[16px] mb-2 leading-4
                      transition-colors duration-200
                      group-hover:text-orange-500
                    "
                  >
                    {listing.title}
                  </h3>
                </div>
              </div>
            ))}

          </div>

          <button
            className="mt-6 mx-auto flex items-center gap-2 text-blue-600 hover:text-blue-700 font-medium"
          >
            <span>Показать больше</span>
            <ChevronRight className="w-5 h-5" />
          </button>
        </div>
      </div>
    </div>
  );
};
export default HomePage;
